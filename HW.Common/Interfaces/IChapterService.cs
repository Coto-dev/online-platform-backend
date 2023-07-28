using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;
using Microsoft.AspNetCore.Mvc;

namespace HW.Common.Interfaces;

public interface IChapterService
{
    Task<ChapterFullDto> GetChapter(Guid chapterId, Guid userId);
    Task LearnChapter(Guid chapterId, Guid userId);
    Task SendComment(ChapterCommentDto message, Guid chapterId);
    Task DeleteComment(Guid commentId, Guid userId);
    Task EditComment(ChapterCommentSendDto message, Guid commentId, Guid userId);

}