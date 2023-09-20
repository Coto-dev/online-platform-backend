using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.AdminPanel.API.Controllers;

/// <summary>
/// Controller for admin
/// </summary>
[ApiController]
//[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Administrator)]
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
    /// Add teacher rights to user [Admin]
    /// </summary>
    [HttpPost]
    [Route("add/teacher/{userId}/to/{moduleId}")]
    public async Task<ActionResult> AddTeacherRightsToUser(Guid userId, Guid moduleId)
    {

        await _adminPanelService.AddTeacherRightsToUser(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Delete teacher rights from user [Admin]
    /// </summary>
    [HttpPost]
    [Route("delete/teacher/{userId}/from/{moduleId}")]
    public async Task<ActionResult> DeleteTeacherRightsFromUser(Guid userId, Guid moduleId)
    {

        await _adminPanelService.DeleteTeacherRightsFromUser(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Add editor rights to user [Admin]
    /// </summary>
    [HttpPost]
    [Route("add/editor/{userId}/to/{moduleId}")]
    public async Task<ActionResult> AddEditorRightsToUser(Guid userId, Guid moduleId)
    {
        await _adminPanelService.AddEditorRightsToUser(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Delete editor rights from user [Admin]
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/delete-teacher/{userId}")]
    public async Task<ActionResult> DeleteEditorRightsFromUser(Guid userId, Guid moduleId)
    {

        await _adminPanelService.DeleteEditorRightsFromUser(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Add student to module [Admin]
    /// </summary>
    [HttpPost]
    [Route("add/student/{userId}/to/{moduleId}")]
    public async Task<ActionResult> AddStudentToModule(Guid userId, Guid moduleId)
    {

        await _adminPanelService.AddStudentToModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Delete student from module [Admin]
    /// </summary>
    [HttpPost]
    [Route("delete/student/{userId}/from/{module}")]
    public async Task<ActionResult> DeleteStudentFromModule(Guid userId, Guid moduleId)
    {

        await _adminPanelService.DeleteStudentFromModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Get exact student's marks on module [Admin]
    /// </summary>
    [HttpGet]
    [Route("marks/{userId}/from/{moduleId}")]
    public async Task<ActionResult> GetStudentMarksFromModule(Guid userId, Guid moduleId)
    {

        await _adminPanelService.GetStudentMarksFromModule(userId, moduleId);
        return Ok();
    }

    /// <summary>
    /// Get all marks of students on module [Admin]
    /// </summary>
    [HttpGet]
    [Route("all-marks/from/{moduleId}")]
    public async Task<ActionResult> GetStudentsMarksFromModule(Guid moduleId)
    {

        await _adminPanelService.GetStudentsMarksFromModule(moduleId);
        return Ok();
    }

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
    [HttpPost]
    [Route("unban/{userId}")]
    public async Task<ActionResult> UnbanUser(Guid userId)
    {

        await _adminPanelService.UnbanUser(userId);
        return Ok();
    }





}