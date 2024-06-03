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
    /// Get all available modules(main page) [Any(unauthorized)]
    /// </summary>
    [HttpGet]
    [Route("available/list")]
    public async Task<ActionResult<PagedList<ModuleShortDto>>> GetAvailableModules(
        [FromQuery] PaginationParamsDto pagination,
        [FromQuery] FilterModuleType? filter,
        [FromQuery] string? sortByNameFilter,
        [FromQuery] ModuleTagsDto? ModuleTags,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            userId = Guid.Empty;
        }
        return Ok(await _moduleStudentService.GetAvailableModules(pagination, filter, sortByNameFilter,
                sortModuleType, userId, ModuleTags));
    }
    

    /// <summary>
    /// Get student modules [Student]
    /// </summary>
    [HttpGet]
    [Route("student/list")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult<PagedList<ModuleShortDto>>> GetStudentModules([FromQuery] PaginationParamsDto pagination,
        [FromQuery] FilterModuleType? filter,
        [FromQuery] string? sortByNameFilter, 
        [FromQuery] ModuleStudentFilter? section,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        return Ok(await _moduleStudentService.GetStudentModules(pagination, filter, sortByNameFilter, section,
            sortModuleType, userId));
    }

    /// <summary>
    /// Get module content by moduleId [Student]
    /// </summary>
    ///<remarks>
    /// return module with submodules 
    /// </remarks>
    [HttpGet]
    [Route("{moduleId}/content")]
    public async Task<ActionResult<ModuleFullDto>> GetModuleContent(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            userId = Guid.Empty;
        }

        /*
        await _checkPermissionService.CheckStudentModulePermission(userId, moduleId);
        */
        return Ok(await _moduleStudentService.GetModuleContent(moduleId, userId));
    }
    
    
    /// <summary>
    /// Get module details by moduleId [Any(unauthorized)]
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
    /// Get module comments by moduleId [Any(unauthorized)]
    /// </summary>
    [HttpGet]
    [Route("{moduleId}/comments")]
    public async Task<ActionResult<PagedList<ModuleCommentDto>>> GetModuleComments([FromQuery] PaginationParamsDto pagination, Guid moduleId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            userId = Guid.Empty;
        }
        return Ok(await _moduleStudentService.GetModuleComments(moduleId, pagination));
    }

    /// <summary>
    /// Send comment to module [Teacher][Student]
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/comment")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student
                                                         + "," + ApplicationRoleNames.Teacher 
                                                         + "," + ApplicationRoleNames.Administrator)]
    public async Task<ActionResult> SendCommentToModule([FromBody] ModuleCommentCreateDto model, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _moduleStudentService.SendCommentToModule(model, moduleId, userId);
        return Ok();
    }

    /// <summary>
    /// Edit comment to module [Teacher][Student]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/comment")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student
                                                         + "," + ApplicationRoleNames.Teacher
                                                         + "," + ApplicationRoleNames.Administrator)]
    public async Task<ActionResult> EditCommentToModule([FromBody] ModuleCommentEditDto model, Guid commentId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _moduleStudentService.EditCommentInModule(model, commentId, userId);
        return Ok();
    }

    /// <summary>
    /// Delete comment from module [Teacher][Student]
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}/comment")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student
                                                         + "," + ApplicationRoleNames.Teacher
                                                         + "," + ApplicationRoleNames.Administrator)]
    public async Task<ActionResult> DeleteCommentInModule(Guid commentId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _moduleStudentService.DeleteCommentFromModule(commentId, userId);
        return Ok();
    }

    /// <summary>
    /// Buy module [Student]
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
    /// Start module [Student]
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/start")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult> StartModule(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _moduleStudentService.StartModule(moduleId, userId);
        return Ok();
    }
    
    /// <summary>
    /// Add module to basket [Student]
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
    /// Delete module from basket [Student]
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

    /// <summary>
    /// Add spent time on module [Student]
    /// </summary>
    [HttpPut]
    [Route("{moduleId}/timespent")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student)]
    public async Task<ActionResult> AddSpentTimeOnModule(Guid moduleId, [FromBody] SpentTimeDto spentTime)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }
        await _moduleStudentService.AddSpentTimeOnModule(moduleId, userId, spentTime);
        return Ok();

    }

    /// <summary>
    /// Get tags list [Any(unauthorized)]
    /// </summary>
    [HttpGet]
    [Route("tags")]
    public async Task<ActionResult<PagedList<TagDto>>> SearchModuleTags(
        [FromQuery] string? tagName,
        [FromQuery] PaginationParamsDto pagination)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            userId = Guid.Empty;
        }

        return Ok(await _moduleStudentService.SearchModuleTags(tagName, pagination));
    }

    /// <summary>
    /// Get tag list of module [Any(unauthorized)]
    /// </summary>
    [HttpGet]
    [Route("tags/{moduleId}")]
    public async Task<ActionResult<List<TagDto>>> GetTagsOfModule(Guid moduleId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            userId = Guid.Empty;
        }

        return Ok(await _moduleStudentService.GetTagsOfModule(moduleId));
    }

}