using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces; 

public interface ITeacherManagerService {
    Task<List<StudentWithWorksDto>> GetStudents(Guid moduleId);
    Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId);
    Task<List<TestForReview>> GetTestsForReview(Guid moduleId, Guid studentId);
}