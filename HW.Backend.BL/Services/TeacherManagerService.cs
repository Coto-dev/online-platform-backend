using System.Collections.Immutable;
using HW.Backend.DAL.Data;
using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HW.Backend.BL.Services; 

public class TeacherManagerService : ITeacherManagerService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;
    private readonly IModuleStudentService _moduleStudentService;
    private readonly IFileService _fileService;
    public TeacherManagerService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext, IModuleStudentService moduleStudentService, IFileService fileService) {
        _logger = logger;
        _dbContext = dbContext;
        _moduleStudentService = moduleStudentService;
        _fileService = fileService;
    }

    public async Task<PagedList<StudentWithWorksDto>> GetStudents(Guid moduleId,
        PaginationParamsDto pagination) {
        var module = _dbContext.Modules
            .FirstOrDefault(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var totalStudents =  _dbContext.Students
            .Where(s => s.Modules != null && s.Modules!.Any(m => m.Module == module))
            .OrderByDescending(s => s.UserAnswerTests!
                .Count(uat => uat.Test.Chapter.SubModule.Module == module
                              && uat.IsLastAttempt
                              && uat.Status == UserAnswerTestStatus.SentToCheck))
            .Select(s => new StudentWithWorksDto {
                Id = s.Id,
                WorksCount = s.UserAnswerTests!
                    .Count(uat => uat.Test.Chapter.SubModule.Module == module
                                  && uat.IsLastAttempt
                                  && uat.Status == UserAnswerTestStatus.SentToCheck)
            });
     var response = await PagedList<StudentWithWorksDto>.ToPagedList(totalStudents, pagination.PageNumber, pagination.PageSize);
     return response;
    }

    public async Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId) {
        var module = await _dbContext.Modules
            .Include(m=>m.SubModules)!
            .ThenInclude(s=>s.Chapters)!
            .ThenInclude(c=>c.ChapterTests)
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(s => s.Id == studentId);
        var gradeGraph = new GradeGraph {
            UserProgress = new UserProgress {
                Id = studentId,
                PassedTests =   await _dbContext.Chapters
                    .Where(c=>!c.ArchivedAt.HasValue
                              && c.ChapterType == ChapterType.TestChapter
                              && c.ChapterTests!.Count != 0
                              && c.ChapterTests
                                  .All(ct=> !ct.ArchivedAt.HasValue 
                              && ct.UserAnswerTests!.Where(uat=>uat.IsLastAttempt)
                                  .All(uat=>uat.Student == student 
                                            && uat.Status == UserAnswerTestStatus.Passed)) 
                              && !c.SubModule.ArchivedAt.HasValue
                              && c.SubModule.Module == module)
                    .CountAsync(),
                TotalChapters = await _dbContext.Chapters
                    .Where(c=>!c.ArchivedAt.HasValue 
                              && !c.SubModule.ArchivedAt.HasValue
                              && c.ChapterType == ChapterType.DefaultChapter
                              && c.SubModule.Module == module)
                    .CountAsync(),
                TotalTests = await _dbContext.Chapters
                    .Where(c=>!c.ArchivedAt.HasValue 
                              && !c.SubModule.ArchivedAt.HasValue
                              && c.ChapterType == ChapterType.TestChapter
                              && c.SubModule.Module == module)
                    .CountAsync(),
                LearnedChapters = await _dbContext.Learned
                    .Where(l=>!l.Chapter.ArchivedAt.HasValue 
                              && !l.Chapter.SubModule.ArchivedAt.HasValue
                              && l.Chapter.SubModule.Module == module 
                              && l.LearnedBy == student)
                    .CountAsync(),
                Progress = await _moduleStudentService.CalculateProgressFloat(moduleId, studentId)
            },
            WorksCount = await _dbContext.DetailedAnswers
                .Where(da=> da.UserAnswerTest.IsLastAttempt
                            && da.UserAnswerTest.Student == student
                            && da.UserAnswerTest.Status == UserAnswerTestStatus.SentToCheck
                            && da.UserAnswerTest.Test.Chapter.SubModule.Module == module)
                .CountAsync(),
            SubModules = module.SubModules!
                .OrderBy(s=> module.OrderedSubModules!.IndexOf(s.Id))
                .Where(s=>!s.ArchivedAt.HasValue)
                .Select(s=> new GradeGraphSubModule {
                SubModuleId = s.Id,
                SubModuleName = s.Name,
                GraphElementStatus = UserAnswerTestStatus.NotDone,
                Chapters = s.Chapters!
                    .OrderBy(c=> module.OrderedSubModules!.IndexOf(c.Id))
                    .Where(c=>!c.ArchivedAt.HasValue)
                    .Select(c=> new GradeGraphChapter { 
                        ChapterType = c.ChapterType, 
                        ChapterId = c.Id, 
                        ChapterName = c.Name,
                        GraphElementStatus = UserAnswerTestStatus.NotDone
                }).ToList(),
            }).ToList()
        };
        var userAnswerTests = await _dbContext.UserAnswerTests
            .Include(uat=>uat.Test)
            .ThenInclude(t=>t.Chapter)
            .Where(uat => uat.Student == student
                          && uat.IsLastAttempt
                          && uat.Test.Chapter.SubModule.Module == module)
            .ToListAsync();
        
        foreach (var gradeGraphSubModule in gradeGraph.SubModules) {
            var learnedChapters = await _dbContext.Learned
                .Include(l=>l.Chapter)
                .Where(l => l.LearnedBy == student &&
                            gradeGraphSubModule.Chapters
                                .Select(c=>c.ChapterId).ToList()
                                .Contains(l.Chapter.Id))
                .ToListAsync();
            foreach (var gradeGraphChapter in gradeGraphSubModule.Chapters) {

                if (gradeGraphChapter.ChapterType == ChapterType.DefaultChapter) {
                    gradeGraphChapter.GraphElementStatus =
                        learnedChapters.Any(lc => lc.Chapter.Id == gradeGraphChapter.ChapterId)
                            ? UserAnswerTestStatus.Passed
                            : UserAnswerTestStatus.NotDone;
                }
                else {
                    var userAnswerTestsInChapter = userAnswerTests
                        .Where(uat => uat.Test.Chapter.Id == gradeGraphChapter.ChapterId).ToList();
                    if (userAnswerTestsInChapter.IsNullOrEmpty()) {
                        gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.NotDone;
                        continue;
                    } 
                    if (userAnswerTestsInChapter
                          .All(uat =>  uat.Status == UserAnswerTestStatus.Passed)) 
                        gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.Passed;
                    else { 
                        if (userAnswerTestsInChapter
                               .Any(uat => uat.Status == UserAnswerTestStatus.SentToCheck)) 
                            gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.SentToCheck;
                        else {
                            gradeGraphChapter.GraphElementStatus = userAnswerTestsInChapter
                                .Any(uat => uat.Status == UserAnswerTestStatus.Fail) 
                                ? UserAnswerTestStatus.Fail 
                                : UserAnswerTestStatus.NotDone;
                        }
                    }
                }
            }
            if (gradeGraphSubModule.Chapters.All(c => c.GraphElementStatus == UserAnswerTestStatus.Passed))
                gradeGraphSubModule.GraphElementStatus = UserAnswerTestStatus.Passed;
            else {
                if (gradeGraphSubModule.Chapters.Any(c => c.GraphElementStatus == UserAnswerTestStatus.SentToCheck))
                    gradeGraphSubModule.GraphElementStatus = UserAnswerTestStatus.SentToCheck;
                else {
                    if (gradeGraphSubModule.Chapters.Any(c => c.GraphElementStatus == UserAnswerTestStatus.Fail))
                        gradeGraphSubModule.GraphElementStatus = UserAnswerTestStatus.Fail;
                    else gradeGraphSubModule.GraphElementStatus = UserAnswerTestStatus.NotDone;
                }
            }
        }
        return gradeGraph;
    }

    public async Task<List<ChapterForReview>> GetTestsForReview(Guid moduleId, Guid studentId) {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(s => s.Id == studentId);
        var chaptersForReview = await _dbContext.DetailedAnswers
            .Include(da=>da.UserAnswerTest)
            .ThenInclude(uat=>uat.Test)
            .Where(da => da.UserAnswerTest.Status == UserAnswerTestStatus.SentToCheck
                         && da.UserAnswerTest.IsLastAttempt
                         && da.UserAnswerTest.Student == student
                         && da.UserAnswerTest.Test.Chapter.SubModule.Module == module)
           .GroupBy(da => da.UserAnswerTest.Test.Chapter)
            .ToListAsync();
        var unique = chaptersForReview.DistinctBy(c=>c.Key.Id).ToList();
            var a = unique.Select(c => new ChapterForReview {
                ChapterName = c.Key.Name,
                TestForReview = c.Key.ChapterTests!
                    .SelectMany(t => t.UserAnswerTests!)
                    .SelectMany(uat => uat.UserAnswers.OfType<DetailedAnswer>())
                    .Select(da => new TestForReview {
                        UserAnswerId = da.Id,
                        Question = da.UserAnswerTest.Test.Question,
                        Files = da.Files,
                        StudentAnswerContent = da.AnswerContent ?? ""
                    })
                    .ToList()
            }).ToList();

              foreach (var testForReview in a
                           .SelectMany(chapterForReview => chapterForReview.TestForReview)) {
                  testForReview.Files?.ForEach(async f=> f = await _fileService.GetFileLink(f));
              }
        return a;
            return null;
    }

    public async Task SetAccuracyToDetailedAnswer(Guid teacherId,Guid userAnswerId, DetailedAnswerAccuracy accuracy) {
        var detailedAnswer = await _dbContext.DetailedAnswers
            .Include(da=>da.UserAnswerTest)
            .FirstOrDefaultAsync(da => da.Id == userAnswerId);
        if (detailedAnswer == null)
            throw new NotFoundException("Answer not found");
        var teacher = await _dbContext.Teachers
            .FirstOrDefaultAsync(t => t.Id == teacherId);
        if (teacher == null)
            throw new NotFoundException("Teacher not found");
        detailedAnswer.UserAnswerTest.Status = accuracy.Accuracy <= 2 
            ? UserAnswerTestStatus.Fail 
            : UserAnswerTestStatus.Passed;
        var existingReviewedTest =
            await _dbContext.ReviewedDetailedTests.FirstOrDefaultAsync(t => t.DetailedAnswer == detailedAnswer);
        if (existingReviewedTest == null) {
            await _dbContext.AddAsync(new ReviewedDetailedTests {
                DetailedAnswer = detailedAnswer,
                ReviewedBy = teacher
            });
        }
        else {
            
            existingReviewedTest.ReviewedBy = teacher;
            existingReviewedTest.ReviewedAt = DateTime.UtcNow;
            _dbContext.Update(existingReviewedTest);
        }
        detailedAnswer.Accuracy = accuracy.Accuracy;
        _dbContext.Update(detailedAnswer);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetNewAttemptForTestChapter(Guid studentId, Guid chapterId) {
        var user = await _dbContext.Students
            .FirstOrDefaultAsync(c => c.Id == studentId);
        if (user == null) 
            throw new NotFoundException("User with this id not found");

        var chapter = await _dbContext.Chapters
            .Include(c=>c.ChapterTests)
            .FirstOrDefaultAsync(c => c.Id == chapterId);
        if (chapter == null)
            throw new NotFoundException("Chapter with this id not found");
        if (chapter.ChapterType != ChapterType.ExamChapter && chapter.ChapterType != ChapterType.TestChapter)
            throw new ForbiddenException("Incorrect chapter type");
        var userAnswerTests = await _dbContext.UserAnswerTests
            .Where(uat => uat.Student == user 
                          && uat.IsLastAttempt
                          && uat.Test.Chapter == chapter)
            .ToListAsync();
        if (userAnswerTests.IsNullOrEmpty())
            throw new ForbiddenException("User does not have any answers");
        var newUserAnswerTests = userAnswerTests.Select(uat => new UserAnswerTest {
            Test = uat.Test,
            UserAnswers = new List<UserAnswer>(),
            NumberOfAttempt = uat.NumberOfAttempt + 1,
            Student = user,
            Status = UserAnswerTestStatus.NotDone,
            IsLastAttempt = true
        }).ToList();
        userAnswerTests.ForEach(uat=>uat.IsLastAttempt = false);
        _dbContext.UpdateRange(userAnswerTests);
        await _dbContext.AddRangeAsync(newUserAnswerTests);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<SpentTimeOnModuleResultDto> GetStudentSpentTimeOnModule(Guid studentId, Guid moduleId)
    {
        var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId);
        if (module == null)
            throw new NotFoundException("Module not found");

        var user = await _dbContext.Students
            .FirstOrDefaultAsync(u => u.Id == studentId);
        if (user == null)
            throw new NotFoundException("User not found");

        var userModule = await _dbContext.UserModules
            .FirstOrDefaultAsync(um => um.Module == module && um.Student == user);
        if (userModule == null)
            throw new ConflictException("User's module not found");

        var response = new SpentTimeOnModuleResultDto
        {
            ModuleId = moduleId,
            StudentId = studentId,
            SpentTimeDto = new SpentTimeDto
            {
                Days = userModule.SpentTime.Days,
                Hours = userModule.SpentTime.Hours,
                Minutes = userModule.SpentTime.Minutes,
                Seconds = userModule.SpentTime.Seconds
            }
        };

        return response;
    }
}