using HW.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for activity management
/// </summary>
[ApiController]
[Route("api/activity")]
public class TeacherManagerController : ControllerBase {


    private readonly ILogger<ChapterController> _logger;
    private readonly ICheckPermissionService _checkPermissionService;
    private readonly IActivityService _activityService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkPermissionService"></param>
    /// <param name="activityService"></param>
    public TeacherManagerController(ILogger<ChapterController> logger, ICheckPermissionService checkPermissionService,
        IActivityService activityService) {
        _logger = logger;
        _checkPermissionService = checkPermissionService;
        _activityService = activityService;
    }
}