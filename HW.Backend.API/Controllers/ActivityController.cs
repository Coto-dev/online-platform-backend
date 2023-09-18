using HW.Backend.BL.Services;
using HW.Common.DataTransferObjects;
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
    /// Get user's activity
    /// </summary>
    [HttpGet]
    [Route("{userId}")]
    public async Task<ActionResult<YearActivityDto>> GetUserActivity(Guid userId)
    {

        return Ok(await _activityService.GetUserActivity(userId));
    }
}