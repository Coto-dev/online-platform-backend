using HW.Common.DataTransferObjects;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for chapter management
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/chapter")]
public class ChapterController : ControllerBase {


    private readonly ILogger<ChapterController> _logger;
    private readonly ICheckPermissionService _checkPermissionService;
    private readonly IChapterService _chapterService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkPermissionService"></param>
    /// <param name="chapterService"></param>
    public ChapterController(ILogger<ChapterController> logger, ICheckPermissionService checkPermissionService, 
        IChapterService chapterService) {
        _logger = logger;
        _checkPermissionService = checkPermissionService;
        _chapterService = chapterService;
    }
    
    /// <summary>
    /// Make chapter learned
    /// </summary>
    [HttpPost]
    [Route("{chapterId}")]
    public async Task<ActionResult> LearnChapter(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentChapterPermission(userId, chapterId);
        await _chapterService.LearnChapter(chapterId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Send comment
    /// </summary>
    [HttpPost]
    [Route("{chapterId}/comment")]
    public async Task<ActionResult> SendComment([FromBody] ChapterCommentDto message, Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentChapterPermission(userId, chapterId);
        await _checkPermissionService.CheckTeacherChapterPermission(userId, chapterId);
        await _chapterService.SendComment(message, chapterId);
        return Ok();
    }

    /// <summary>
    /// Delete comment
    /// </summary>
    [HttpDelete]
    [Route("comment/{commentId}")]
    public async Task<ActionResult> DeleteComment(Guid commentId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _chapterService.DeleteComment(commentId, userId);
        return Ok();
    }

    /// <summary>
    /// Edit comment
    /// </summary>
    [HttpPut]
    [Route("comment/{commentId}")]
    public async Task<ActionResult> EditComment([FromBody] ChapterCommentSendDto message, Guid commentId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }


        await _chapterService.EditComment(message, commentId, userId);
        return Ok();
    }
}