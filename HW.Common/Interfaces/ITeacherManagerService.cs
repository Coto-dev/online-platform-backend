using HW.Common.DataTransferObjects;
using HW.Common.Other;

namespace HW.Common.Interfaces; 

public interface ITeacherManagerService {
    Task<PagedList<StudentWithWorksDto>> GetStudents(Guid moduleId, PaginationParamsDto pagination);
    Task<GradeGraph> GetStudentGradeGraph(Guid moduleId, Guid studentId);
    Task<List<ChapterForReview>> GetTestsForReview(Guid moduleId, Guid studentId);
    Task SetAccuracyToDetailedAnswer(Guid teacherId, Guid userAnswerId, DetailedAnswerAccuracy accuracy);
    Task SetNewAttemptForTestChapter(Guid studentId, Guid chapterId);
}