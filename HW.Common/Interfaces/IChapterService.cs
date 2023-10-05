using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces;

public interface IChapterService {
    Task AnswerChapter(Guid chapterId, Guid userId);
    Task LearnChapter(Guid chapterId, Guid userId);
    Task SendComment(ChapterCommentDto message, Guid chapterId);
    Task DeleteComment(Guid commentId, Guid userId);
    Task EditComment(ChapterCommentSendDto message, Guid commentId, Guid userId);
    Task<Guid> CreateChapter(Guid subModuleId, ChapterCreateDto model);
    Task EditChapter(Guid chapterId, ChapterEditDto model);
    Task ArchiveChapter(Guid chapterId);
    Task EditChaptersOrder(List<Guid> orderedChapters, Guid subModuleId);
    Task<ChapterFullDto> GetChapterContentStudent(Guid chapterId, Guid userId);
    Task<ChapterFullTeacherDto> GetChapterContentTeacher(Guid chapterId, Guid userId);

}