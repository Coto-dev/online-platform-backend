using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for chapter management
/// </summary>
[ApiController]
[Route("api")]
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
    /// Get chapter content by chapterId [Editor]
    /// </summary>
    [HttpGet]
    [Route("chapter/{chapterId}/teacher")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher)]

    public async Task<ActionResult<ChapterFullTeacherDto>> GetChapterContent(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        return Ok(await _chapterService.GetChapterContentTeacher(chapterId, userId));
    }
    
    /// <summary>
    /// Get chapter content by chapterId [Student]
    /// </summary>
    [HttpGet]
    [Route("chapter/{chapterId}")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]

    public async Task<ActionResult<ChapterFullDto>> GetChapterContentStudent(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckStudentChapterPermission(userId, chapterId);
        return Ok(await _chapterService.GetChapterContentStudent(chapterId, userId));
    }
    
    /// <summary>
    /// Edit order of chapters [Editor]
    /// </summary>
    [HttpPut]
    [Route("sub-module/{subModuleId}/chapters/order")]
    public async Task<ActionResult> EditChaptersOrder([FromBody] List<Guid> orderedChapters, Guid subModuleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _chapterService.EditChaptersOrder(orderedChapters , subModuleId);
        return Ok();
    }
    
    /// <summary>
    /// Create chapter in sub module [Editor]
    /// </summary>
    [HttpPost]
    [Route("sub-module/{subModuleId}/chapter")]
    public async Task<ActionResult<Guid>> CreateChapter(Guid subModuleId, [FromBody] ChapterCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        return Ok(await _chapterService.CreateChapter(subModuleId, model));
    }
    
    /// <summary>
    /// Edit chapter(save changes) [Editor]
    /// </summary>
    [HttpPut]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> EditChapter(Guid chapterId, [FromBody] ChapterEditDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        await _chapterService.EditChapter(chapterId, model);
        return Ok();
    }
    
    /// <summary>
    /// Archive chapter [Editor]
    /// </summary>
    [HttpDelete]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> ArchiveChapter(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        await _chapterService.ArchiveChapter(chapterId);
        return Ok();
    }
    
    /// <summary>
    /// Make chapter learned [Student]
    /// </summary>
    [HttpPost]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> LearnChapter(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckStudentChapterPermission(userId, chapterId);
        await _chapterService.LearnChapter(chapterId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Send comment [Student][Teacher]
    /// </summary>
    [HttpPost]
    [Route("chapter/{chapterId}/comment")]
    public async Task<ActionResult> SendComment([FromBody] ChapterCommentDto message, Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        if (User.IsInRole(ApplicationRoleNames.Teacher)) 
            await _checkPermissionService.CheckTeacherChapterPermission(userId, chapterId);
        else if (User.IsInRole(ApplicationRoleNames.Student))
            await _checkPermissionService.CheckStudentChapterPermission(userId, chapterId);
        await _chapterService.SendComment(message, chapterId);
        return Ok();
    }

    /// <summary>
    /// Delete comment [Comment's owner]
    /// </summary>
    [HttpDelete]
    [Route("chapter/comment/{commentId}")]
    public async Task<ActionResult> DeleteComment(Guid commentId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _chapterService.DeleteComment(commentId, userId);
        return Ok();
    }

    /// <summary>
    /// Edit comment [Comment's owner]
    /// </summary>
    [HttpPut]
    [Route("chapter/comment/{commentId}")]
    public async Task<ActionResult> EditComment([FromBody] ChapterCommentSendDto message, Guid commentId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }
        await _chapterService.EditComment(message, commentId, userId);
        return Ok();
    }
}