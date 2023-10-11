using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Interfaces;
using HW.Common.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.AdminPanel.API.Controllers; 

/// <summary>
/// Controller for admin and teacher
/// </summary>
[ApiController]
[Route("api/admin-panel")]
public class TeacherAccessController : ControllerBase
{

    private readonly ILogger<AdminPanelController> _logger;
    private readonly IAdminPanelService _adminPanelService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="adminPanelService"></param>
    public TeacherAccessController(ILogger<AdminPanelController> logger,
        IAdminPanelService adminPanelService)
    {
        _logger = logger;
        _adminPanelService = adminPanelService;
    }

    /// <summary>
    /// Get users list [Admin][Teacher]
    /// </summary>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer",
        Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("users/list")]
    public async Task<ActionResult<PagedList<UserShortDto>>> GetUsers([FromQuery] PaginationParamsDto pagination,
        [FromQuery] string? searchString,
        [FromQuery] FilterRoleType? filter,
        [FromQuery] Guid? moduleId,
        [FromQuery] SearchType? sortUserType = SearchType.FullName) {
        var a = User;
        return Ok(await _adminPanelService.GetUsers(pagination, filter, sortUserType, searchString, moduleId));
    }
}