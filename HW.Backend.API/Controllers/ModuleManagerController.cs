using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers; 

/// <summary>
/// Controller for teacher module management
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
[Route("api/module")]
public class ModuleManagerController : ControllerBase {
   

    private readonly ILogger<ModuleManagerController> _logger;
    private readonly IModuleManagerService _moduleManagerService;
    private readonly ICheckPermissionService _checkPermissionService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkPermissionService"></param>
    /// <param name="moduleManagerService"></param>
    public ModuleManagerController(ILogger<ModuleManagerController> logger, ICheckPermissionService checkPermissionService,
        IModuleManagerService moduleManagerService) {
        _logger = logger;
        _checkPermissionService = checkPermissionService;
        _moduleManagerService = moduleManagerService;
    }
    
    /// <summary>
    /// Get teacher's created modules [Teacher]
    /// </summary>
    [HttpGet]
    [Route("teacher/list")]
    public async Task<ActionResult<PagedList<ModuleShortDto>>> GetTeacherModules([FromQuery] PaginationParamsDto pagination, 
        [FromQuery] FilterModuleType? filter, 
        [FromQuery] string? sortByNameFilter, 
        [FromQuery] ModuleFilterTeacherType? section,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _moduleManagerService.GetTeacherModules(pagination, filter, section, sortByNameFilter, sortModuleType, userId));
    }
    
    /// <summary>
    /// Get module content by moduleId [Teacher]
    /// </summary>
    ///<remarks>
    /// return module with submodules 
    /// </remarks>
    [HttpGet]
    [Route("{moduleId}/content/teacher")]
    public async Task<ActionResult<ModuleFullTeacherDto>> GetModuleContent(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        return Ok(await _moduleManagerService.GetModuleContent(moduleId, userId));
    }
    
    /// <summary>
    /// Get chapter content by chapterId [Teacher]
    /// </summary>
    ///<remarks>
    /// 
    /// </remarks>
    [HttpGet]
    [Route("chapter/{chapterId}/teacher")]
    public async Task<ActionResult<ChapterFullTeacherDto>> GetChapterContent(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        return Ok(await _moduleManagerService.GetChapterContent(chapterId, userId));
    }
    
    /// <summary>
    /// Create self-study module [Teacher]
    /// </summary>
    [HttpPost]
    [Route("self-study")]
    public async Task<ActionResult> CreateSelfStudyModule([FromBody] ModuleSelfStudyCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _moduleManagerService.CreateSelfStudyModule(model, userId);
        return Ok();
    }
    
    /// <summary>
    /// Edit order of sub modules[Teacher]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/sub-modules/order")]
    public async Task<ActionResult> EditSubModulesOrder([FromBody] List<Guid> orderedSubModules, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditSubModulesOrder(orderedSubModules , moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Edit self-study module [Teacher]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/self-study")]
    public async Task<ActionResult> EditSelfStudyModule([FromBody] ModuleSelfStudyEditDto model, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditSelfStudyModule(model, moduleId, userId);
        return Ok();
    }
    /// <summary>
    /// Edit module visibility [Teacher]
    /// </summary>
    [HttpPatch]
    [Route("{moduleId}/visibility")]
    public async Task<ActionResult> EditVisibilityModule([FromQuery] ModuleVisibilityType visibilityType, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditVisibilityModule(visibilityType, moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Create streaming module [Teacher]
    /// </summary>
    [HttpPost]
    [Route("streaming")]
    public async Task<ActionResult> CreateStreamingModule([FromBody] ModuleStreamingCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        await _moduleManagerService.CreateStreamingModule(model, userId);
        return Ok();
    }
    
    /// <summary>
    /// Edit streaming module [Teacher]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/streaming")]
    public async Task<ActionResult> EditStreamingModule([FromBody] ModuleStreamingEditDto model, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditStreamingModule(model, moduleId,userId);
        return Ok();    
    }
    
    /// <summary>
    /// Archive my module [Teacher]
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}")]
    public async Task<ActionResult> ArchiveModule(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.ArchiveModule(moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Create sub module [Teacher]
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/sub-module")]
    public async Task<ActionResult> AddSubModule(Guid moduleId, [FromBody] SubModuleCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.AddSubModule(moduleId, model);
        return Ok();
    }
    
    /// <summary>
    /// Edit order of chapters[Teacher]
    /// </summary>
    [HttpPut]
    [Route("sub-module/{subModuleId}/chapters/order")]
    public async Task<ActionResult> EditChaptersOrder([FromBody] List<Guid> orderedChapters, Guid subModuleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _moduleManagerService.EditChaptersOrder(orderedChapters , subModuleId);
        return Ok();
    }
    
    /// <summary>
    /// Edit sub module [Teacher]
    /// </summary>
    [HttpPut]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> EditSubModule(Guid subModuleId, [FromBody] SubModuleEditDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _moduleManagerService.EditSubModule(subModuleId, model);
        return Ok();
    }
    
    /// <summary>
    /// Archive sub module [Teacher]
    /// </summary>
    [HttpDelete]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> ArchiveSubModule(Guid subModuleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _moduleManagerService.ArchiveSubModule(subModuleId);
        return Ok();
    }
    
    /// <summary>
    /// Create chapter in sub module [Teacher]
    /// </summary>
    [HttpPost]
    [Route("sub-module/{subModuleId}/chapter")]
    public async Task<ActionResult> CreateChapter(Guid subModuleId, [FromBody] ChapterCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _moduleManagerService.CreateChapter(subModuleId, model);
        return Ok();
    }
    
    /// <summary>
    /// Edit chapter(save changes) [Teacher]
    /// </summary>
    [HttpPut]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> EditChapter(Guid chapterId, [FromBody] ChapterEditDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        await _moduleManagerService.EditChapter(chapterId, model);
        return Ok();
    }
    
    /// <summary>
    /// Archive chapter [Teacher]
    /// </summary>
    [HttpDelete]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> ArchiveChapter(Guid chapterId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorChapterPermission(userId, chapterId);
        await _moduleManagerService.ArchiveChapter(chapterId);
        return Ok();
    }
    
}