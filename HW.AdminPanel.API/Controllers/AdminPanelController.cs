using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.AdminPanel.API.Controllers;

/// <summary>
/// Controller for admin
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Administrator)]
[Route("api/admin-panel")]
public class AdminPanelController : ControllerBase
{

    private readonly ILogger<AdminPanelController> _logger;
    private readonly IAdminPanelService _adminPanelService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="adminPanelService"></param>
    public AdminPanelController(ILogger<AdminPanelController> logger,
        IAdminPanelService adminPanelService)
    {
        _logger = logger;
        _adminPanelService = adminPanelService;
    }

    /// <summary>
    /// Get users list [Admin]
    /// </summary>
    [HttpGet]
    [Route("users/list")]
    public async Task<ActionResult<PagedList<UserShortDto>>> GetUsers([FromQuery] PaginationParamsDto pagination,
        [FromQuery] string? searchString,
        [FromQuery] FilterRoleType? filter,
        [FromQuery] SearchType? sortUserType = SearchType.FullName)
    {
        return Ok(await _adminPanelService.GetUsers(pagination, filter, sortUserType,searchString));
    }

    /// <summary>
    /// Get all modules list [Admin]
    /// </summary>
    [HttpGet]
    [Route("modules/list")]
    public async Task<ActionResult<PagedList<ModuleShortAdminDto>>> GetModules([FromQuery] PaginationParamsDto pagination,
        [FromQuery] FilterModuleType? filter,
        [FromQuery] string? sortByNameFilter,
        [FromQuery] SortModuleType? sortModuleType = SortModuleType.NameAsc,
        [FromQuery] bool withArchived = true)
    {
        return Ok(await _adminPanelService.GetModules(pagination, filter, sortByNameFilter, sortModuleType, withArchived));
    }

    /// <summary>
    /// Add teacher rights to user [Admin]
    /// </summary>
    [HttpPost]
    [Route("teacher/{userId}")]
    public async Task<ActionResult> AddTeacherRightsToUser(Guid userId)
    {
        await _adminPanelService.AddTeacherRightsToUser(userId);
        return Ok();
    }

    /// <summary>
    /// Delete teacher rights to user [Admin]
    /// </summary>
    [HttpDelete]
    [Route("teacher/{userId}")]
    public async Task<ActionResult> DeleteTeacherRightsFromUser(Guid userId)
    {
        await _adminPanelService.DeleteTeacherRightsFromUser(userId);
        return Ok();

    }


    /// <summary>
    /// Add teacher rights to user in module [Admin]
    /// </summary>
    [HttpPost]
    [Route("teacher/{userId}/to/{moduleId}")]
    public async Task<ActionResult> AddTeacherRightsToUserOnModule(Guid userId, Guid moduleId)
    {
        await _adminPanelService.AddTeacherRightsToUserOnModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Delete teacher rights from user on module [Admin]
    /// </summary>
    [HttpDelete]
    [Route("teacher/{userId}/from/{moduleId}")]
    public async Task<ActionResult> DeleteTeacherRightsFromUserOnModule(Guid userId, Guid moduleId)
    {
        await _adminPanelService.DeleteTeacherRightsFromUserOnModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Add editor rights to user on module [Admin]
    /// </summary>
    [HttpPost]
    [Route("editor/{userId}/to/{moduleId}")]
    public async Task<ActionResult> AddEditorRightsToUserOnModule(Guid userId, Guid moduleId)
    {
        await _adminPanelService.AddEditorRightsToUserOnModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Delete editor rights from user on module [Admin]
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}/delete-teacher/{userId}")]
    public async Task<ActionResult> DeleteEditorRightsFromUserOnModule(Guid userId, Guid moduleId)
    {
        await _adminPanelService.DeleteEditorRightsFromUserOnModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Add student to module [Admin]
    /// </summary>
    [HttpPost]
    [Route("student/{userId}/to/{moduleId}")]
    public async Task<ActionResult> AddStudentToModule(Guid userId, Guid moduleId)
    {
        await _adminPanelService.AddStudentToModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Delete student from module [Admin]
    /// </summary>
    [HttpDelete]
    [Route("student/{userId}/from/{moduleId}")]
    public async Task<ActionResult> DeleteStudentFromModule(Guid userId, Guid moduleId)
    {
        await _adminPanelService.DeleteStudentFromModule(userId, moduleId);
        return Ok();
    }

    /*    /// <summary>
        /// Get exact student's marks on module [Admin]
        /// </summary>
        [HttpGet]
        [Route("marks/{userId}/on/{moduleId}")]
        public async Task<ActionResult> GetStudentMarksFromModule(Guid userId, Guid moduleId)
        {

            throw new NotImplementedException();
        }*/

    /*    /// <summary>
        /// Get all marks of students on module [Admin]
        /// </summary>
        [HttpGet]
        [Route("all-marks/on/{moduleId}")]
        public async Task<ActionResult> GetStudentsMarksFromModule(Guid moduleId)
        {

            throw new NotImplementedException();
        }*/

    /// <summary>
    /// Ban user [Admin]
    /// </summary>
    [HttpPost]
    [Route("ban/{userId}")]
    public async Task<ActionResult> BanUser(Guid userId)
    {
        await _adminPanelService.BanUser(userId);
        return Ok();
    }

    /// <summary>
    /// Unban user [Admin]
    /// </summary>
    [HttpDelete]
    [Route("unban/{userId}")]
    public async Task<ActionResult> UnbanUser(Guid userId)
    {
        await _adminPanelService.UnbanUser(userId);
        return Ok();
    }
}