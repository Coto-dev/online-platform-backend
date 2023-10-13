using HW.Backend.DAL.Data;
using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services; 

public class TeacherManagerService : ITeacherManagerService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;

    public TeacherManagerService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<StudentWithWorksDto>> GetStudents(Guid moduleId) {
        var module = _dbContext.Modules
            .FirstOrDefault(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var moduleStudents = _dbContext.UserModules
            .Where(m => m.Module == module)
            .Select(um => um.Student);
        
        var totalStudents = await _dbContext.DetailedAnswers
            .Where(t => t.Accuracy == 0 && moduleStudents.Contains(t.UserAnswerTest.Student)
                                        && t.UserAnswerTest.AnsweredAt.HasValue
                                        && t.UserAnswerTest.Test.Chapter.SubModule.Module == module)
            .GroupBy(t => t.UserAnswerTest.Student.Id)
            .Select(t=> new StudentWithWorksDto {
                Id = t.Key,
                WorksCount = t.Count()
            })
            .ToListAsync();
        return totalStudents;
    }

    public async Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId) {
        throw new NotImplementedException();
    }

    public async Task<List<TestForReview>> GetTestsForReview(Guid moduleId, Guid studentId) {
        throw new NotImplementedException();
    }

    public async Task SetAccuracyToDetailedAnswer(Guid studentId, Guid teacherId,Guid userAnswerId, DetailedAnswerAccuracy accuracy) {
        throw new NotImplementedException();
    }

    public async Task SetNewAttemptForTestChapter(Guid studentId, Guid chapterId) {
        throw new NotImplementedException();
    }
}