using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for module management
/// </summary>
[ApiController]
[Route("api/module")]
public class ModuleStudentController : ControllerBase {
   

    private readonly ILogger<ModuleStudentController> _logger;
    private readonly IModuleStudentService _moduleStudentService;
    private readonly ICheckPermissionService _checkPermissionService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="moduleStudentService"></param>
    /// <param name="checkPermissionService"></param>
    public ModuleStudentController(ILogger<ModuleStudentController> logger, IModuleStudentService moduleStudentService,
        ICheckPermissionService checkPermissionService) {
        _logger = logger;
        _moduleStudentService = moduleStudentService;
        _checkPermissionService = checkPermissionService;
    }


    /// <summary>
    /// Get all available modules(main page)
    /// </summary>
    [HttpGet]
    [Route("available/list")]
    public async Task<ActionResult<PagedList<ModuleShortDto>>> GetAvailableModules(
        [FromQuery] PaginationParamsDto pagination,
        [FromQuery] FilterModuleType? filter,
        [FromQuery] string? sortByNameFilter,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            userId = Guid.Empty;
        }
        return Ok(await _moduleStudentService.GetAvailableModules(pagination, filter, sortByNameFilter,
                sortModuleType, userId));
    }
    

    /// <summary>
    /// Get student modules
    /// </summary>
    [HttpGet]
    [Route("student/list")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult<PagedList<ModuleShortDto>>> GetStudentModules([FromQuery] PaginationParamsDto pagination,
        [FromQuery] FilterModuleType? filter,
        [FromQuery] string? sortByNameFilter, 
        [FromQuery] ModuleFilterStudentType? section = ModuleFilterStudentType.InProcess,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _moduleStudentService.GetStudentModules(pagination, filter, sortByNameFilter, section,
            sortModuleType, userId));
    }

    /// <summary>
    /// Get module content by moduleId
    /// </summary>
    ///<remarks>
    /// return module with submodules 
    /// </remarks>
    [HttpGet]
    [Route("{moduleId}/content")]
    public async Task<ActionResult<ModuleFullDto>> GetModuleContent(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get chapter content by chapterId
    /// </summary>
    ///<remarks>
    /// 
    /// </remarks>
    [HttpGet]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult<ChapterFullDto>> GetChapterContent(Guid chapterId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get module details by moduleId
    /// </summary>
    [HttpGet]
    [Route("{moduleId}/details")]
    public async Task<ActionResult<ModuleDetailsDto>> GetModuleDetails(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            userId = Guid.Empty;
        }
        return Ok(await _moduleStudentService.GetModuleDetails(moduleId, userId));
    }
    
    /// <summary>
    /// Send comment to module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/comment")]
    [Authorize(AuthenticationSchemes = "Bearer", 
        Roles = ApplicationRoleNames.Student
                + "," + ApplicationRoleNames.Teacher 
                + "," + ApplicationRoleNames.Administrator)]
    public async Task<ActionResult> SendCommentToModule([FromBody] ModuleCommentDto model, Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Buy module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult> BuyModule(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _moduleStudentService.BuyModule(moduleId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Add module to basket
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/basket")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult> AddModuleToBasket(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _moduleStudentService.AddModuleToBasket(moduleId, userId);
        return Ok();    
    }
    
    /// <summary>
    /// Delete module from basket
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}/basket")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult> DeleteModuleFromBasket(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _moduleStudentService.DeleteModuleFromBasket(moduleId, userId);
        return Ok();
        
    }
}