using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for activity management
/// </summary>
[ApiController]
[Route("api/teacher")]
public class TeacherManagerController : ControllerBase {


    private readonly ILogger<ChapterController> _logger;
    private readonly ICheckPermissionService _checkPermissionService;
    private readonly ITeacherManagerService _teacherManagerService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="checkPermissionService"></param>
    /// <param name="teacherManagerService"></param>
    public TeacherManagerController(ILogger<ChapterController> logger,
        ICheckPermissionService checkPermissionService, ITeacherManagerService teacherManagerService) {
        _logger = logger;
        _checkPermissionService = checkPermissionService;
        _teacherManagerService = teacherManagerService;
    }
    
    /// <summary>
    /// Get students with works
    /// </summary>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("module/{moduleId}/student/list")]
    public async Task<ActionResult<List<StudentWithWorksDto>>> GetStudents(Guid moduleId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckTeacherModulePermission(userId, moduleId);
        return Ok(await _teacherManagerService.GetStudents(moduleId));
    }
    
    /// <summary>
    /// Get students grade graph with progress
    /// </summary>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("module/{moduleId}/student/{studentId}/graph")]
    public async Task<ActionResult<GradeGraph>> GetStudentGradeGraph(Guid moduleId, Guid studentId){
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckTeacherModulePermission(userId, moduleId);
        return Ok(await _teacherManagerService.GetStudentGradeGraph(moduleId, studentId));
    }
    
    /// <summary>
    /// Get student's tests for review
    /// </summary>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("module/{moduleId}/student/{studentId}/test-review")]
    public async Task<ActionResult<List<ChapterForReview>>> GetTestsForReview(Guid moduleId, Guid studentId){
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckTeacherModulePermission(userId, moduleId);
        return Ok(await _teacherManagerService.GetTestsForReview(moduleId, studentId));
    }
    
    /// <summary>
    /// Set accuracy to student's detailed answer
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("user-answer/{userAnswerId}/detailed-test/accuracy")]
    public async Task<ActionResult> SetAccuracyToDetailedAnswer(Guid userAnswerId,  DetailedAnswerAccuracy accuracy){
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckTeacherUserAnswerPermission(userId, userAnswerId);
        await _teacherManagerService.SetAccuracyToDetailedAnswer(userId, userAnswerId, accuracy);
        return Ok();
    }
    
    /// <summary>
    /// Give student new attempt for tests in chapter
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("student/{studentId}/test-chapter/{chapterId}")]
    public async Task<ActionResult> SetNewAttemptForTestChapter(Guid studentId, Guid chapterId){
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false) {
            throw new UnauthorizedException("User is not authorized");
        }
        if (!User.IsInRole(ApplicationRoleNames.Administrator))
            await _checkPermissionService.CheckTeacherChapterPermission(userId, chapterId);
        await _teacherManagerService.SetNewAttemptForTestChapter(studentId, chapterId);
        return Ok();
    }
}