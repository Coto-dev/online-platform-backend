using System.Collections.Immutable;
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

    public async Task<List<StudentWithWorksDto>> GetStudents(Guid moduleId) {
        var module = _dbContext.Modules
            .FirstOrDefault(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var moduleStudents = await _dbContext.UserModules
            .Where(m => m.Module == module)
            .Select(um => um.Student).ToListAsync();
        var totalStudents = await _dbContext.DetailedAnswers
            .Where(t => moduleStudents.Contains(t.UserAnswerTest.Student)
                                        && t.UserAnswerTest.Test.Chapter.SubModule.Module == module)
            .GroupBy(t => t.UserAnswerTest.Student.Id)
            .Select(t=> new StudentWithWorksDto {
                Id = t.Key,
                WorksCount = t.Count(da => da.Accuracy == 0 
                                          && da.UserAnswerTest.AnsweredAt.HasValue)
            })
            .ToListAsync();
         totalStudents.AddRange(moduleStudents
            .Where(ms=>totalStudents
                .All(ts=>ts.Id != ms.Id)).Select(ms=>new StudentWithWorksDto {
                Id = ms.Id,
                WorksCount = 0
            }).ToList());
         return totalStudents;
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
                PassedTests =  _dbContext.UserAnswerTests
                    .Where(uat=> uat.Status == UserAnswerTestStatus.Passed
                                 && uat.IsLastAttempt
                                 && uat.Student == student
                                && uat.Test.Chapter.SubModule.Module == module)
                    .GroupBy(uat=>uat.Test.Chapter)
                    .Count(),
                TotalChapters = await _dbContext.Chapters
                    .Where(c=>!c.ArchivedAt.HasValue 
                              && c.ChapterType == ChapterType.DefaultChapter
                              && c.SubModule.Module == module)
                    .CountAsync(),
                TotalTests = await _dbContext.Chapters
                    .Where(c=>!c.ArchivedAt.HasValue 
                              && c.ChapterType == ChapterType.TestChapter
                              && c.SubModule.Module == module)
                    .CountAsync(),
                LearnedChapters = await _dbContext.Learned
                    .Where(l=>!l.Chapter.ArchivedAt.HasValue 
                              && l.Chapter.SubModule.Module == module 
                              && l.LearnedBy == student)
                    .CountAsync(),
                Progress = await _moduleStudentService.CalculateProgressFloat(moduleId, studentId)
            },
            WorksCount = await _dbContext.DetailedAnswers
                .Where(da=>da.Accuracy == 0
                && da.UserAnswerTest.Student == student
                && da.UserAnswerTest.Test.Chapter.SubModule.Module == module)
                .CountAsync(),
            SubModules = module.SubModules!
                .OrderBy(s=> module.OrderedSubModules!.IndexOf(s.Id))
                .Where(s=>!s.ArchivedAt.HasValue)
                .Select(s=> new GradeGraphSubModule {
                SubModuleId = s.Id,
                SubModuleName = s.Name,
                Chapters = s.Chapters!
                    .OrderBy(c=> module.OrderedSubModules!.IndexOf(c.Id))
                    .Where(c=>!c.ArchivedAt.HasValue)
                    .Select(c=> new GradeGraphChapter { 
                        ChapterType = c.ChapterType, 
                        ChapterId = c.Id, 
                        ChapterName = c.Name,
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
                        learnedChapters.Any(lc=>lc.Chapter.Id == gradeGraphChapter.ChapterId) 
                            ? UserAnswerTestStatus.Passed 
                            : UserAnswerTestStatus.NotDone;
                }
                else
               if (userAnswerTests
                   .Where(uat=>uat.Test.Chapter.Id == gradeGraphChapter.ChapterId)
                   .All(uat => uat.Status == UserAnswerTestStatus.Passed))
                   gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.Passed;
               else {
                   if (userAnswerTests
                       .Where(uat=>uat.Test.Chapter.Id == gradeGraphChapter.ChapterId)
                       .Any(uat => uat.Status == UserAnswerTestStatus.SentToCheck))
                       gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.SentToCheck;
                   else {
                       if (userAnswerTests
                           .Where(uat=>uat.Test.Chapter.Id == gradeGraphChapter.ChapterId)
                           .Any(uat => uat.Status == UserAnswerTestStatus.Fail))
                           gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.Fail;
                       else gradeGraphChapter.GraphElementStatus = UserAnswerTestStatus.NotDone;
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
            .Where(da => da.UserAnswerTest.Status == UserAnswerTestStatus.SentToCheck
                          && da.UserAnswerTest.Student == student && da.UserAnswerTest.Test.Chapter.SubModule.Module == module)
            .Select(da=> new ChapterForReview() {
                ChapterName = da.UserAnswerTest.Test.Chapter.Name,
                TestForReview = new TestForReview(){
                UserAnswerId = da.Id,
                Question = da.UserAnswerTest.Test.Question,
                Files =  da.Files,
                StudentAnswerContent = da.AnswerContent ?? ""
            }}).ToListAsync();
        chaptersForReview.ForEach(async t=>t.TestForReview.Files = t.TestForReview.Files?.Select(f=> _fileService.GetFileLink(f).Result).ToList());
        return chaptersForReview;
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
}