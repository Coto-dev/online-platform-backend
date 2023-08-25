using HW.Common.DataTransferObjects;

namespace HW.Common.Interfaces;

public interface IChapterService
{
    Task LearnChapter(Guid chapterId, Guid userId);
    Task SendComment(ChapterCommentDto message, Guid chapterId);
    Task DeleteComment(Guid commentId, Guid userId);
    Task EditComment(ChapterCommentSendDto message, Guid commentId, Guid userId);

}