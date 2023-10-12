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
       
        /*var module = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId && !m.ArchivedAt.HasValue);
        if (module == null)
            throw new NotFoundException("Module not found");
        var WorksCount = _dbContext.DetailedAnswers
            .Where(da => da.Accuracy != 0 
                         && da.UserAnswerTest.Student == user
                         && da.UserAnswerTest.Test.Chapter.SubModule.Module == module)
            .CountAsync();*/
        throw new NotImplementedException();

    }

    public async Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId) {
        throw new NotImplementedException();
    }

    public async Task<List<TestForReview>> GetTestsForReview(Guid moduleId, Guid studentId) {
        throw new NotImplementedException();
    }
}