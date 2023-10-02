using HW.Backend.DAL.Data;
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
            .Include(c=>c.ChapterTests)
            .Include(c=>c.ChapterBlocks)
            .Include(c=>c.ChapterComments)!
            .ThenInclude(com=>com.User)
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        var user = await _dbContext.Students
            .Include(u=>u.LearnedChapters)
            .FirstOrDefaultAsync(u => u.Id == userId);
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
                    Content = SwapFileIdsWithUrls(cb.Content, cb.Files).Result,
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
                    PossibleSimpleAnswers = t is SimpleAnswerTest { PossibleAnswers: not null } simpleAnswerTest
                        ? simpleAnswerTest.PossibleAnswers
                        .Select(uat=> new SimpleAnswerDto {
                            Id = uat.Id,
                            AnswerContent = uat.AnswerContent,
                            isRight = uat.IsRight
                        }).ToList():new List<SimpleAnswerDto>(),
                    PossibleCorrectSequenceAnswers = t is CorrectSequenceTest { PossibleAnswers: not null } correctSequenceTest
                        ? correctSequenceTest.PossibleAnswers
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
            .FirstOrDefaultAsync(m => m.Id == chapterId);
        var user = await _dbContext.Students
            .Include(u=>u.LearnedChapters)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return new ChapterFullDto {
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
                    // FilesUrls = cb.Files.IsNullOrEmpty()
                    //     ? new List<string>()
                    //     : cb.Files!.Select(async f=> await _fileService.GetFileLink(f)).Select(task=>task.Result).ToList()!
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
                        SimpleAnswerTest simpleTest => simpleTest.PossibleAnswers.IsNullOrEmpty() 
                            ? new List<PossibleAnswerDto>() 
                            : simpleTest.PossibleAnswers
                            .Select(pa => new PossibleAnswerDto {
                                Id = pa.Id,
                                AnswerContent = pa.AnswerContent
                            }).ToList(),
                        CorrectSequenceTest correctSequenceTest =>
                            correctSequenceTest.PossibleAnswers.IsNullOrEmpty() 
                            ? new List<PossibleAnswerDto>()
                            : correctSequenceTest.PossibleAnswers
                            .Select(pa => new PossibleAnswerDto {
                                Id = pa.Id,
                                AnswerContent = pa.AnswerContent
                            }).ToList(),
                        _ => new List<PossibleAnswerDto>()
                    },
                    UserAnswer = 
                        _dbContext.UserAnswerTests.Any(uat=>uat.Student == user && uat.Test == t) ?
                            t.TestType is TestType.ExtraAnswer 
                        or TestType.MultipleAnswer 
                        or TestType.SingleAnswer 
                        or TestType.MultipleExtraAnswer ? new UserAnswerFullDto {
                        UserAnswerSimples = _dbContext.UserAnswers.OfType<SimpleUserAnswer>()
                            .Where(u=>u.UserAnswerTest.Test == t && u.UserAnswerTest.Student == user)
                            .Select(s=> new UserAnswerSimpleDto {
                                Id = s.SimpleAnswer.Id
                            }).ToList(),
                        IsAnswered =  _dbContext.UserAnswerTests
                            .Where(uat=>uat.Student == user && uat.Test == t)
                            .MaxBy(uat=>uat.NumberOfAttempt)!.AnsweredAt.HasValue 
                    } : t.TestType is TestType.CorrectSequenceAnswer ? new UserAnswerFullDto {
                                UserAnswerCorrectSequences = _dbContext.UserAnswers.OfType<CorrectSequenceUserAnswer>()
                                .Where(u=>u.UserAnswerTest.Test == t && u.UserAnswerTest.Student == user)
                                .Select(s=> new UserAnswerCorrectSequenceDto() {
                                Id = s.CorrectSequenceAnswer.Id,
                                Order = s.Order
                                }).ToList(),
                                IsAnswered = _dbContext.UserAnswerTests
                                    .Where(uat=>uat.Student == user && uat.Test == t)
                                    .MaxBy(uat=>uat.NumberOfAttempt)!.AnsweredAt.HasValue 
                            } : t.TestType is TestType.DetailedAnswer ? new UserAnswerFullDto {
                                DetailedAnswer = _dbContext.UserAnswers.OfType<DetailedAnswer>()
                                    .FirstOrDefault(u=>u.UserAnswerTest.Test == t && u.UserAnswerTest.Student == user)!
                                    .AnswerContent,
                                IsAnswered = _dbContext.UserAnswerTests
                                    .Where(uat=>uat.Student == user && uat.Test == t)
                                    .MaxBy(uat=>uat.NumberOfAttempt)!.AnsweredAt.HasValue
                            } : null : null,
                    Type = t.TestType,
                }).ToList()
        };
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