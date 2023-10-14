using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces; 

public interface ITeacherManagerService {
    Task<List<StudentWithWorksDto>> GetStudents(Guid moduleId);
    Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId);
    Task<List<ChapterForReview>> GetTestsForReview(Guid moduleId, Guid studentId);
    Task SetAccuracyToDetailedAnswer(Guid teacherId, Guid userAnswerId, DetailedAnswerAccuracy accuracy);
    Task SetNewAttemptForTestChapter(Guid studentId, Guid chapterId);
}