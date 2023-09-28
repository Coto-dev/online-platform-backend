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
        [FromQuery] ModuleTeacherFilter? section,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _moduleManagerService.GetTeacherModules(pagination, filter, section, sortByNameFilter, sortModuleType, userId));
    }
    
    /// <summary>
    /// Get module content by moduleId [Editor]
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
        if (!User.IsInRole(ApplicationRoleNames.Administrator)) 
            await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        return Ok(await _moduleManagerService.GetModuleContent(moduleId, userId));
    }

    /// <summary>
    /// Create self-study module [Teacher]
    /// </summary>
    [HttpPost]
    [Route("self-study")]
    public async Task<ActionResult<Guid>>  CreateSelfStudyModule([FromBody] ModuleSelfStudyCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _moduleManagerService.CreateSelfStudyModule(model, userId));
    }

    /// <summary>
    /// Edit self-study module [Editor]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/self-study")]
    public async Task<ActionResult> EditSelfStudyModule([FromBody] ModuleSelfStudyEditDto model, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditSelfStudyModule(model, moduleId, userId);
        return Ok();
    }
    /// <summary>
    /// Edit module visibility [Editor]
    /// </summary>
    [HttpPatch]
    [Route("{moduleId}/visibility")]
    public async Task<ActionResult> EditVisibilityModule([FromQuery] ModuleVisibilityType visibilityType, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditVisibilityModule(visibilityType, moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Create streaming module [Teacher]
    /// </summary>
    [HttpPost]
    [Route("streaming")]
    public async Task<ActionResult<Guid>>  CreateStreamingModule([FromBody] ModuleStreamingCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }

        return Ok(await _moduleManagerService.CreateStreamingModule(model, userId));
    }
    
    /// <summary>
    /// Edit streaming module [Editor]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/streaming")]
    public async Task<ActionResult> EditStreamingModule([FromBody] ModuleStreamingEditDto model, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditStreamingModule(model, moduleId,userId);
        return Ok();    
    }
    
    /// <summary>
    /// Archive my module [Editor]
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}")]
    public async Task<ActionResult> ArchiveModule(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.ArchiveModule(moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Edit order of full module structure [Editor]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/full-structure/order")]
    public async Task<ActionResult> EditModulesOrder([FromBody]SortStructureDto structureDto, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _moduleManagerService.EditModuleSortStructure(structureDto , moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Add editor to module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/editor")]
    public async Task<ActionResult> AddEditorToModule([FromQuery] Guid userId, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid authorId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckAuthorModulePermission(authorId, moduleId);
        await _moduleManagerService.AddEditorToModule(userId, moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Remove editor from module
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}/editor")]
    public async Task<ActionResult> RemoveEditorFromModule([FromQuery] Guid userId, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid authorId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckAuthorModulePermission(authorId, moduleId);
        await _moduleManagerService.RemoveEditorFromModule(userId, moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Add teacher to module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/teacher")]
    public async Task<ActionResult> AddTeacherToModule([FromQuery] Guid userId, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid editorId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(editorId, moduleId);
        await _moduleManagerService.AddTeacherToModule(userId, moduleId);
        return Ok();
    }
    
    /// <summary>
    /// Remove teacher from module
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}/teacher")]
    public async Task<ActionResult> RemoveTeacherFromModule([FromQuery] Guid userId, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid editorId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckCreatorModulePermission(editorId, moduleId);
        await _moduleManagerService.RemoveTeacherFromModule(userId, moduleId);
        return Ok();
    }
}