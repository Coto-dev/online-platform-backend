using HW.Backend.DAL.Data;
using HW.Common.DataTransferObjects;
using HW.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HW.Backend.BL.Services; 

public class TeacherManagerService : ITeacherManagerService {
    private readonly ILogger<ModuleManagerService> _logger;
    private readonly BackendDbContext _dbContext;

    public TeacherManagerService(ILogger<ModuleManagerService> logger, BackendDbContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<StudentWithWorksDto>> GetStudents(Guid moduleId, Guid userId) {
        throw new NotImplementedException();
    }

    public async Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId) {
        throw new NotImplementedException();
    }

    public async Task<List<TestForReview>> GetTestsForReview(Guid moduleId, Guid studentId) {
        throw new NotImplementedException();
    }
}