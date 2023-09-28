using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers; 

/// <summary>
/// ChapterBlocksController
/// </summary>
[ApiController] 
[Route("api")]
[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher)]
public class ChapterBlocksController: ControllerBase {
    private readonly IChapterBlocksService _chapterBlocksService;
    private readonly ICheckPermissionService _checkPermissionService;

    /// <summary>
    /// const
    /// </summary>
    /// <param name="chapterBlocksService"></param>
    /// <param name="checkPermissionService"></param>
    public ChapterBlocksController(IChapterBlocksService chapterBlocksService, ICheckPermissionService checkPermissionService) {
        _chapterBlocksService = chapterBlocksService;
        _checkPermissionService = checkPermissionService;
    }
    /// <summary>
    /// Create chapter block [Editor]
    /// </summary>
    /// <param name="chapterId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("chapter/{chapterId}/chapter-block")]
    public async Task<ActionResult<Guid>> CreateChapterBlock(Guid chapterId,[FromBody] ChapterBlockCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        return Ok(await _chapterBlocksService.CreateChapterBlock(chapterId, model));
        
    }
    
    /// <summary>
    /// Edit chapter block [Editor]
    /// </summary>
    /// <param name="chapterBlockId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("chapter-block/{chapterBlockId}")]
    public async Task<ActionResult> EditChapterBlock(Guid chapterBlockId, [FromBody] ChapterBlockEditDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterBlockPermission(userId, chapterBlockId);
        await _chapterBlocksService.EditChapterBlock(chapterBlockId, model);
        return Ok();
    }
    
    /// <summary>
    /// Archive chapter block [Editor]
    /// </summary>
    /// <param name="chapterBlockId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("chapter-block/{chapterBlockId}")]
    public async Task<ActionResult> ArchiveChapterBlock(Guid chapterBlockId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterBlockPermission(userId, chapterBlockId);
        await _chapterBlocksService.ArchiveChapterBlock(chapterBlockId);
        return Ok();
    }
    
    /// <summary>
    /// Edit order of chapter blocks [Editor]
    /// </summary>
    [HttpPut]
    [Route("chapter/{chapterId}/chapter-blocks/order")]
    public async Task<ActionResult> EditChapterBlocksOrder(Guid chapterId,[FromBody] List<Guid> orderedChapterBlocks) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        await _chapterBlocksService.EditChapterBlocksOrder(orderedChapterBlocks , chapterId);
        return Ok();
    }
    
}