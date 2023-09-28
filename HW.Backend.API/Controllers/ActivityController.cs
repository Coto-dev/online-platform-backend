using HW.Backend.BL.Services;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for activity management
/// </summary>
[ApiController]
[Route("api/activity")]
public class ActivityController : ControllerBase
{


    private readonly ILogger<ChapterController> _logger;
    private readonly ICheckPermissionService _checkPermissionService;
    private readonly IActivityService _activityService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkPermissionService"></param>
    /// <param name="activityService"></param>
    public ActivityController(ILogger<ChapterController> logger, ICheckPermissionService checkPermissionService,
        IActivityService activityService)
    {
        _logger = logger;
        _checkPermissionService = checkPermissionService;
        _activityService = activityService;
    }

    /// <summary>
    /// Get student's activity [Student][Teacher]
    /// </summary>
    [HttpGet]
    [Route("{studentId}")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Student
                                                         + "," + ApplicationRoleNames.Teacher
                                                         + "," + ApplicationRoleNames.Administrator)]
    public async Task<ActionResult<YearActivityDto>> GetUserActivity(Guid studentId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }


        return Ok(await _activityService.GetUserActivity(studentId));
    }
}