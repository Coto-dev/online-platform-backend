﻿using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HW.Backend.BL.Services;

public class ChapterService : IChapterService
{
    private readonly ILogger<ChapterService> _logger;
    private readonly BackendDbContext _dbContext;
    private readonly IFileService _fileService;


    public ChapterService(ILogger<ChapterService> logger, BackendDbContext dbContext, IFileService fileService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _fileService = fileService;
    }

    public async Task AnswerChapter(Guid chapterId, Guid userId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(c => c.Id == userId);
        if (user == null) 
            throw new NotFoundException("User with this id not found");

        var chapter = await _dbContext.Chapters
            .Include(c=>c.ChapterTests)
            .FirstOrDefaultAsync(c => c.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter with this id not found");
        if (chapter.ChapterType != ChapterType.ExamChapter && chapter.ChapterType != ChapterType.TestChapter)
            throw new ForbiddenException("Incorrect chapter type");
        
        var userAnswers = await _dbContext.UserAnswerTests
            .Include(uat=>uat.Test)
            .Include(ua=>ua.UserAnswers)
            .Where(uat => chapter.ChapterTests!.Contains(uat.Test)
                          && uat.Student == user
                          && uat.IsLastAttempt)
            .ToListAsync();
        if (userAnswers.Any(uat => uat.AnsweredAt.HasValue))
            throw new ForbiddenException("Already answered");
        if (userAnswers.Where(uat=>!uat.UserAnswers.IsNullOrEmpty()).ToList().Count < chapter.ChapterTests!.Count(ct => !ct.ArchivedAt.HasValue))
            throw new ForbiddenException($"User answers: {userAnswers.Where(uat=>!uat.UserAnswers.IsNullOrEmpty()).ToList().Count}, but tests: {chapter.ChapterTests!.Count(ct => !ct.ArchivedAt.HasValue)}");
        
        foreach (var userAnswerTest in userAnswers) {
            userAnswerTest.AnsweredAt = DateTime.UtcNow;
            switch (userAnswerTest.Test.TestType) {
                case TestType.SingleAnswer:
                    var rightAnswer = await _dbContext.SimpleAnswers
                        .FirstOrDefaultAsync(sa => sa.SimpleAnswerTest == userAnswerTest.Test && sa.IsRight);
                    userAnswerTest.Status = userAnswerTest.UserAnswers.OfType<SimpleUserAnswer>()
                        .All(ua => rightAnswer == ua.SimpleAnswer) ? UserAnswerTestStatus.Passed : UserAnswerTestStatus.Fail;
                    break;
                case TestType.MultipleAnswer:
                    var rightAnswers = await _dbContext.SimpleAnswers
                        .Where(sa => sa.SimpleAnswerTest == userAnswerTest.Test && sa.IsRight)
                        .ToListAsync();
                    if (userAnswerTest.UserAnswers.OfType<SimpleUserAnswer>().Count() == rightAnswers.Count &&
                        userAnswerTest.UserAnswers.OfType<SimpleUserAnswer>()
                            .All(ua => rightAnswers.Contains(ua.SimpleAnswer)))
                        userAnswerTest.Status = UserAnswerTestStatus.Passed;
                    else userAnswerTest.Status = UserAnswerTestStatus.Fail;
                    break;
                case TestType.CorrectSequenceAnswer:
                    var rightCorrectSequenceAnswers = await _dbContext.CorrectSequenceAnswers
                        .Where(csa => csa.CorrectSequenceTest == userAnswerTest.Test)
                        .ToListAsync();
                    if (userAnswerTest.UserAnswers.OfType<CorrectSequenceUserAnswer>().Count() == rightCorrectSequenceAnswers.Count &&
                        userAnswerTest.UserAnswers.OfType<CorrectSequenceUserAnswer>()
                            .All(ua => rightCorrectSequenceAnswers.Any(ra=>
                                ra.RightOrder == ua.Order
                                && ra == ua.CorrectSequenceAnswer)))
                        userAnswerTest.Status = UserAnswerTestStatus.Passed;
                    else userAnswerTest.Status = UserAnswerTestStatus.Fail;
                    break;
                case TestType.DetailedAnswer:
                    userAnswerTest.Status = UserAnswerTestStatus.SentToCheck;
                    break;
                default:
                    userAnswerTest.Status = UserAnswerTestStatus.NotDone;
                    continue;
                    break;
            }
        }
        _dbContext.UpdateRange(userAnswers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task LearnChapter(Guid chapterId, Guid userId)
    {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(c => c.Id == userId);

        if (user == null) 
            throw new NotFoundException("User with this id not found");

        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new NotFoundException("Chapter with this id not found");

        var learnedChapter = await _dbContext.Learned
            .FirstOrDefaultAsync(c => c.LearnedBy == user && c.Chapter == chapter);

        if (learnedChapter != null && learnedChapter.LearnDate.HasValue)
            throw new BadRequestException("User already learned this chapter");

        await _dbContext.AddAsync(new Learned
        {
            LearnedBy = user,
            Chapter = chapter,
            LearnDate = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task SendComment(ChapterCommentDto message, Guid chapterId)
    {
        var user = await _dbContext.UserBackends
            .FirstOrDefaultAsync(c => c.Id == message.UserId);

        if (user == null)
            throw new NotFoundException("User with this id not found");

        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new NotFoundException("Chapter with this id not found");

        await _dbContext.AddAsync(new ChapterComment
        {
            Id = Guid.NewGuid(),
            Chapter = chapter,
            User = user,
            IsTeacherComment = message.IsTeacherComment,
            Comment = message.Message
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteComment(Guid commentId, Guid userId)
    {
        var comment = await _dbContext.ChapterComments
            .FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null)
            throw new NotFoundException("Comment with this id not found");

        var user = await _dbContext.UserBackends
            .FirstOrDefaultAsync (c => c.Id == userId);
        if (user == null)
            throw new NotFoundException("User with this id not found");

        if (comment.User != user)
            throw new ConflictException("User have not rules to delete this comment");

        var chapter = comment.Chapter;

        _dbContext.ChapterComments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditComment(ChapterCommentSendDto message, Guid commentId, Guid userId)
    {
        var comment = await _dbContext.ChapterComments
            .FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment == null)
            throw new NotFoundException("Comment with this id not found");

        var user = await _dbContext.UserBackends
            .FirstOrDefaultAsync(c => c.Id == userId);
        if (user == null)
            throw new NotFoundException("User with this id not found");

        if (comment.User != user)
            throw new ConflictException("User have not rules to delete this comment");

        comment.Comment = message.Message;
        _dbContext.Update(comment);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<ChapterFullTeacherDto> GetChapterContentTeacher(Guid chapterId, Guid userId) {
        var chapter = await _dbContext.Chapters
            .Include(c=>c.ChapterBlocks)
            .Include(c=>c.ChapterComments)!
            .ThenInclude(com=>com.User)
            .Include(c=>c.ChapterTests)
            .FirstOrDefaultAsync(m => m.Id == chapterId && !m.ArchivedAt.HasValue);
        
        if (chapter == null)
            throw new NotFoundException("Chapter nor found");
        
        var response = new ChapterFullTeacherDto {
            Id = chapter!.Id,
            Name = chapter.Name,
            Content = chapter.Content ?? "",
            FileIds = chapter.Files == null
                ? new List<FileLinkDto>()
                : chapter.Files.Select( f => new FileLinkDto {
                    FileId = f,
                    Url = null 
                }).ToList(),
            Comments = chapter.ChapterComments == null
                ? new List<ChapterCommentDto>()
                : chapter.ChapterComments
                    .Select(com => new ChapterCommentDto {
                    Id = com.Id,
                    UserId = com.User.Id,
                    IsTeacherComment = com.IsTeacherComment,
                    Message = com.Comment
                }).ToList(),
            ChapterType = chapter.ChapterType,
            ChapterBlocks = chapter.ChapterBlocks == null
                ? new List<ChapterBlockTeacherDto>()
                : chapter.ChapterBlocks
                    .Where(cb=>!cb.ArchivedAt.HasValue)
                    .OrderBy(cb=> chapter.OrderedBlocks!.IndexOf(cb.Id))
                    .Select(cb=> new ChapterBlockTeacherDto {
                        Id = cb.Id,
                    Content = cb.Content,
                    FileIds = cb.Files == null
                        ? new List<FileLinkDto>()
                        : cb.Files.Select( f => new FileLinkDto {
                            FileId = f,
                            Url = null 
                        }).ToList()
                }).ToList(),
            Tests = chapter.ChapterTests == null
                ? new List<TestTeacherDto>()
                : chapter.ChapterTests
                    .Where(t=>!t.ArchivedAt.HasValue)
                    .OrderBy(t=> chapter.OrderedTests!.IndexOf(t.Id))
                    .Select(t => new TestTeacherDto {
                    Id = t.Id,
                    Question = t.Question,
                    FileIds = t.Files == null
                        ? new List<FileLinkDto>()
                        : t.Files.Select( f => new FileLinkDto {
                            FileId = f,
                            Url = null 
                        }).ToList(),
                    PossibleSimpleAnswers = t is SimpleAnswerTest
                        ? _dbContext.SimpleAnswerTests.Include(s=>s.PossibleAnswers)
                            .FirstOrDefaultAsync(x=>x.Id == t.Id).Result?.PossibleAnswers
                            .OrderBy(s=>s.CreatedAt)
                        .Select(uat=> new SimpleAnswerDto {
                            Id = uat.Id,
                            AnswerContent = uat.AnswerContent,
                            isRight = uat.IsRight
                        }).ToList():new List<SimpleAnswerDto>(),
                    PossibleCorrectSequenceAnswers = t is CorrectSequenceTest
                        ? _dbContext.CorrectSequenceTest.Include(s=>s.PossibleAnswers)
                            .FirstOrDefaultAsync(x=>x.Id == t.Id).Result?.PossibleAnswers
                            .OrderBy(s=>s.RightOrder)
                            .ThenBy(s=>s.CreatedAt)
                            .Select(uat=> new CorrectSequenceAnswerDto {
                            Id = uat.Id,
                            AnswerContent = uat.AnswerContent,
                            RightOrder = uat.RightOrder
                        }).ToList():new List<CorrectSequenceAnswerDto>(),
                    Type = t.TestType,
                }).ToList()
        };
        foreach (var responseFileId in response.FileIds) {
            responseFileId.Url = (await _fileService.GetFileLink(responseFileId.FileId!) ?? null)!;
        }
        foreach (var fileLinkDto in response.Tests.SelectMany(testTeacherDto => testTeacherDto.FileIds!)) {
            fileLinkDto.Url = (await _fileService.GetFileLink(fileLinkDto.FileId!) ?? null)!;
        }
        foreach (var fileLinkDto in response.ChapterBlocks.SelectMany(testTeacherDto => testTeacherDto.FileIds!)) {
            fileLinkDto.Url = (await _fileService.GetFileLink(fileLinkDto.FileId!) ?? null)!;
        }
        return response;
    }
    
     public async Task<ChapterFullDto> GetChapterContentStudent(Guid chapterId, Guid userId) {
        var chapter = await _dbContext.Chapters
            .Include(c=>c.ChapterTests)
            .Include(c=>c.ChapterBlocks)
            .Include(c=>c.ChapterComments)!
            .ThenInclude(com=>com.User)
            .FirstOrDefaultAsync(m => m.Id == chapterId && !m.ArchivedAt.HasValue);
        if (chapter == null)
            throw new NotFoundException("Chapter nor found");
        var user = await _dbContext.Students
            .Include(u=>u.LearnedChapters)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException("User nor found");
        var response =  new ChapterFullDto {
            Id = chapter!.Id,
            Name = chapter.Name,
            Content = chapter.Content ?? "",
            FileUrls = chapter.Files.IsNullOrEmpty()
                ? new List<string>()
                : chapter.Files!.Select(async f=> await _fileService.GetFileLink(f)).Select(task=>task.Result).ToList()!, 
             Comments = chapter.ChapterComments == null  
                ? new List<ChapterCommentDto>()
                : chapter.ChapterComments.Select(com => new ChapterCommentDto {
                    Id = com.Id,
                    UserId = com.User.Id,
                    IsTeacherComment = com.IsTeacherComment,
                    Message = com.Comment
                }).ToList(),
            IsLearned = user!.LearnedChapters != null && user.LearnedChapters.Any(l=>l.Chapter == chapter),
            ChapterType = chapter.ChapterType,
            ChapterBlocks = chapter.ChapterBlocks == null
                ? new List<ChapterBlockDto>()
                : chapter.ChapterBlocks
                    .Where(cb=>!cb.ArchivedAt.HasValue)
                    .OrderBy(cb=> chapter.OrderedBlocks!.IndexOf(cb.Id))
                    .Select(cb=> new ChapterBlockDto {
                    Content = SwapFileIdsWithUrls(cb.Content, cb.Files).Result,
                    Id = cb.Id
                    }).ToList(),
            Tests = chapter.ChapterTests == null
                ? new List<TestDto>()
                : chapter.ChapterTests
                    .Where(t=>!t.ArchivedAt.HasValue)
                    .OrderBy(t=> chapter.OrderedTests!.IndexOf(t.Id))
                    .Select(t => new TestDto {
                    Id = t.Id,
                    Question = t.Question,
                    FileUrls = t.Files.IsNullOrEmpty()
                        ? new List<string>()
                        : t.Files!.Select(async f=> await _fileService.GetFileLink(f)).Select(task=>task.Result).ToList()!, 
                    PossibleAnswers = t switch {
                        SimpleAnswerTest simpleTest => _dbContext.SimpleAnswerTests.Include(s=>s.PossibleAnswers)
                            .FirstOrDefaultAsync(x=>x.Id == t.Id).Result?.PossibleAnswers
                            .Select(pa => new PossibleAnswerDto {
                                Id = pa.Id,
                                AnswerContent = pa.AnswerContent,
                                IsRight = pa.IsRight
                            }).ToList(),
                        CorrectSequenceTest correctSequenceTest =>
                            _dbContext.CorrectSequenceTest.Include(s=>s.PossibleAnswers)
                                .FirstOrDefaultAsync(x=>x.Id == t.Id).Result?.PossibleAnswers
                                .Select(pa => new PossibleAnswerDto {
                                Id = pa.Id,
                                AnswerContent = pa.AnswerContent,
                                RightOrder = pa.RightOrder
                            }).ToList(),
                        _ => new List<PossibleAnswerDto>()
                    },
                    UserAnswer = 
                        _dbContext.UserAnswerTests.Any(uat=>uat.Student == user 
                                                            && uat.IsLastAttempt
                                                            && uat.Test == t) ?
                            t.TestType is TestType.MultipleAnswer 
                        or TestType.SingleAnswer 
                         ? new UserAnswerFullDto {
                        UserAnswerSimples = _dbContext.UserAnswers.OfType<SimpleUserAnswer>()
                            .Where(u=>u.UserAnswerTest.Test == t 
                                      && u.UserAnswerTest.IsLastAttempt
                                      && u.UserAnswerTest.Student == user)
                            .Select(s=> s.SimpleAnswer.Id).ToList(),
                        IsAnswered =  _dbContext.UserAnswerTests
                            .FirstOrDefault(uat=>uat.Student == user 
                                                 && uat.IsLastAttempt
                                                 && uat.Test == t)!.AnsweredAt.HasValue 
                    } : t.TestType is TestType.CorrectSequenceAnswer ? new UserAnswerFullDto {
                                UserAnswerCorrectSequences = _dbContext.UserAnswers.OfType<CorrectSequenceUserAnswer>()
                                .Where(u=>u.UserAnswerTest.Test == t 
                                          && u.UserAnswerTest.IsLastAttempt
                                          && u.UserAnswerTest.Student == user)
                                .OrderBy(u=>u.Order)
                                .Select(s=> new UserAnswerCorrectSequenceDto() {
                                Id = s.CorrectSequenceAnswer.Id,
                                AnswerContent = s.CorrectSequenceAnswer.AnswerContent,
                                Order = s.Order
                                }).ToList(),
                                IsAnswered = _dbContext.UserAnswerTests
                                    .FirstOrDefault(uat=>uat.Student == user 
                                                         && uat.IsLastAttempt
                                                         && uat.Test == t)!.AnsweredAt.HasValue 
                            } : t.TestType is TestType.DetailedAnswer ? new UserAnswerFullDto {
                                DetailedAnswer = _dbContext.UserAnswers.OfType<DetailedAnswer>()
                                    .Where(u=>u.UserAnswerTest.Test == t 
                                              && u.UserAnswerTest.IsLastAttempt
                                              && u.UserAnswerTest.Student == user)!
                                    .AsEnumerable()
                                    .Select(d=> new DetailedAnswerFullDto() {
                                        AnswerContent = d.AnswerContent,
                                        Accuracy = d.Accuracy,
                                        Files = d.Files == null
                                            ? new List<FileLinkDto>()
                                            : d.Files.Select(f => new FileLinkDto {
                                                FileId = f,
                                                Url = null 
                                            }).ToList()
                                    }).FirstOrDefault(),
                                IsAnswered = _dbContext.UserAnswerTests
                                    .FirstOrDefault(uat=>uat.Student == user 
                                                && uat.IsLastAttempt
                                                && uat.Test == t)!.AnsweredAt.HasValue
                         } : null : null,
                    Type = t.TestType,
                }).ToList()
        };
       // Сортировка возможных ответов в том порядке который выбрал пользователь
       /*foreach (var responseTest in response.Tests.Where(t=>t is { Type: TestType.CorrectSequenceAnswer, UserAnswer: not null })) {
           responseTest.PossibleAnswers = responseTest.PossibleAnswers?
               .OrderBy(pa => responseTest.UserAnswer?
                   .UserAnswerCorrectSequences?.FirstOrDefault(x=>x.Id == pa.Id)?.Order)
               .ToList();
       }*/

       response.IsCanCheckAnswer = response.Tests.All(t => t.UserAnswer != null 
                                                           && (!t.UserAnswer.UserAnswerCorrectSequences.IsNullOrEmpty()
                                                               || t.UserAnswer.DetailedAnswer != null
                                                               || !t.UserAnswer.UserAnswerSimples.IsNullOrEmpty()));
       response.IsAnswered = response.Tests.All(t => t.UserAnswer != null) 
                             && response.Tests.All(t => t.UserAnswer!.IsAnswered);
      
       foreach (var responseTest in response.Tests) {
           if (responseTest.Type == TestType.CorrectSequenceAnswer 
               || (responseTest.UserAnswer?.UserAnswerCorrectSequences == null
               && responseTest.PossibleAnswers?.Count > responseTest.UserAnswer?.UserAnswerCorrectSequences?.Count)) {
               var count = responseTest.UserAnswer?.UserAnswerCorrectSequences?.Count ?? 0;
               responseTest.UserAnswer?.UserAnswerCorrectSequences?
                   .AddRange(responseTest.PossibleAnswers?
                       .Where(pa => responseTest.UserAnswer.UserAnswerCorrectSequences
                           .All(ua => ua.Id != pa.Id))
                       .Select(pa => new UserAnswerCorrectSequenceDto {
                           Id = pa.Id,
                           Order = count += 1,
                           AnswerContent = pa.AnswerContent
                       })!);
           }
           responseTest.UserAnswer?.DetailedAnswer?.Files?
               .ForEach(async f=>await _fileService.GetFileLink(f.FileId!));
           
           if (response.IsAnswered) 
               responseTest.PossibleAnswers = responseTest.PossibleAnswers?
               .OrderBy(pa => pa.RightOrder)
               .ToList();
           else
           {
               responseTest.PossibleAnswers?.ForEach(pa=> {
                   pa.RightOrder = null;
                   pa.IsRight = null;
               });
           }
       }
       return response;
     }

     private async Task<string?> SwapFileIdsWithUrls(string? content, List<string>? FileIds) {
         if (content == null) return content;
         foreach (var fileId in FileIds!) {
             var fileUrl = await _fileService.GetFileLink(fileId);
              content = content.Replace(fileId, fileUrl);
         }
         return content;
     }
     
    public async Task EditChaptersOrder(List<Guid> orderedChapters, Guid subModuleId) {
        var duplicates = orderedChapters.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new BadRequestException("There are duplicates: " + string.Join(", ", duplicates.Select(x => x)));
        var subModule = await _dbContext.SubModules
            .Include(m=>m.Chapters)
            .FirstOrDefaultAsync(m => m.Id == subModuleId);
        if (subModule == null)
            throw new NotFoundException("Sub module not found");
        if (subModule.Chapters.IsNullOrEmpty() || subModule.Chapters!.All(c => c.ArchivedAt.HasValue))
            throw new ConflictException("There are no existing chapters");
        var missingChapters = subModule.Chapters!
            .Where(c=>!c.ArchivedAt.HasValue)
            .Select(c=>c.Id)
            .Except(orderedChapters)
            .ToList();
        if (missingChapters.Any())
            throw new ConflictException("These chapters are missing: " 
                                        + string.Join(", ", missingChapters.Select(x => x)));
        var notExistingChapters = orderedChapters.Except(subModule.Chapters!
                .Where(c=>!c.ArchivedAt.HasValue)
                .Select(c=>c.Id)
                .ToList())
            .ToList();
        if (notExistingChapters.Any())
            throw new ConflictException("These chapters do not exist: " 
                                        + string.Join(", ", notExistingChapters.Select(x => x)));
        subModule.OrderedChapters = orderedChapters;
        _dbContext.Update(subModule);
        await _dbContext.SaveChangesAsync();    
    }
    
    public async Task<Guid> CreateChapter(Guid subModuleId, ChapterCreateDto model) {
        var subModule = await _dbContext.SubModules
            .FirstOrDefaultAsync(m => m.Id == subModuleId && !m.ArchivedAt.HasValue);
        if (subModule == null) 
            throw new NotFoundException("Sub module not found");
        var chapter = new Chapter {
            Name = model.Name,
            Content = model.Content,
            SubModule = subModule,
            ChapterType = model.ChapterType,
            Files = model.FileIds
        };
        subModule.OrderedChapters!.Add(chapter.Id);
        await _dbContext.AddAsync(chapter);
        await _dbContext.SaveChangesAsync();
        return chapter.Id;
    }

    public async Task EditChapter(Guid chapterId, ChapterEditDto model) {
        var chapter = await _dbContext.Chapters
            .FirstOrDefaultAsync(m => m.Id == chapterId && !m.ArchivedAt.HasValue);
        if (chapter == null) 
            throw new NotFoundException("Chapter not found");
        chapter.Name = model.Name;
        chapter.Content = model.Content;
        chapter.ChapterType = model.ChapterType;
        chapter.Files = model.FileIds;
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ArchiveChapter(Guid chapterId) {
        var chapter = await _dbContext.Chapters
            .Include(c=>c.SubModule)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        if (chapter == null) 
            throw new NotFoundException("Chapter not found");
        if (chapter.ArchivedAt.HasValue) 
            throw new ConflictException("Already archived");
        chapter.ArchivedAt = DateTime.UtcNow;
        chapter.SubModule.OrderedChapters!.Remove(chapterId);
        _dbContext.Update(chapter);
        await _dbContext.SaveChangesAsync();    
    }
}