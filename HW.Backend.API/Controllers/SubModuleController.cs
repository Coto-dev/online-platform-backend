using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers; 

/// <summary>
/// SubModuleController
/// </summary>
[ApiController]
[Route("api")]
[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher)]
public class SubModuleController : ControllerBase {
    private readonly ICheckPermissionService _checkPermissionService;
    private readonly ISubModuleService _subModuleService;

    /// <summary>
    /// const
    /// </summary>
    /// <param name="checkPermissionService"></param>
    /// <param name="subModuleService"></param>
    public SubModuleController(ICheckPermissionService checkPermissionService, ISubModuleService subModuleService) {
        _checkPermissionService = checkPermissionService;
        _subModuleService = subModuleService;
    }
    
    /// <summary>
    /// Create sub module [Editor]
    /// </summary>
    [HttpPost]
    [Route("module/{moduleId}/sub-module")]
    public async Task<ActionResult<Guid>> AddSubModule(Guid moduleId, [FromBody] SubModuleCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        return Ok(await _subModuleService.AddSubModule(moduleId, model));
    }
    
    /// <summary>
    /// Edit sub module [Editor]
    /// </summary>
    [HttpPut]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> EditSubModule(Guid subModuleId, [FromBody] SubModuleEditDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _subModuleService.EditSubModule(subModuleId, model);
        return Ok();
    }
    
    /// <summary>
    /// Archive sub module [Editor]
    /// </summary>
    [HttpDelete]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> ArchiveSubModule(Guid subModuleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorSubModulePermission(userId, subModuleId);
        await _subModuleService.ArchiveSubModule(subModuleId);
        return Ok();
    }
    
    /// <summary>
    /// Edit order of sub modules [Editor]
    /// </summary>
    [HttpPut]
    [Route("module/{moduleId}/sub-modules/order")]
    public async Task<ActionResult> EditSubModulesOrder([FromBody] List<Guid> orderedSubModules, Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        await _checkPermissionService.CheckCreatorModulePermission(userId, moduleId);
        await _subModuleService.EditSubModulesOrder(orderedSubModules , moduleId);
        return Ok();
    }
}