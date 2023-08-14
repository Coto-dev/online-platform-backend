using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for test management
/// </summary>
[ApiController]
[Route("api/test")]
public class TestController : ControllerBase {

    private readonly ILogger<TestController> _logger;
    private readonly ITestService _testService;
    private readonly ICheckPermissionService _checkPermissionService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="testService"></param>
    /// <param name="checkPermissionService"></param>
    public TestController(ILogger<TestController> logger, ITestService testService, 
        ICheckPermissionService checkPermissionService) {
        _logger = logger;
        _testService = testService;
        _checkPermissionService = checkPermissionService;
    }
    
    /// <summary>
    /// Add simple test to chapter
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/simple")]
    public async Task<ActionResult> AddSimpleTestToChapter(Guid chapterId, [FromBody] TestSimpleCreateDto testModel) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorChapterPermission(chapterId, userId);
        await _testService.AddSimpleTestToChapter(chapterId, testModel);
        return Ok();
    }

    /// <summary>
    /// Edit simple test 
    /// </summary>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("{testId}/simple")]
    public async Task<ActionResult> EditTest(Guid testId, [FromBody] EditTestDto testModel) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.EditTest(testId, testModel);
        return Ok();
    }
    
    /// <summary>
    /// Save answer for simple test type
    /// </summary>
    [HttpPut]
    [Route("{testId}/simple/save")]
    public async Task<ActionResult> SaveAnswerSimpleTest(Guid testId, [FromBody] UserAnswerSimpleDto userAnswers) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentTestPermission(userId, testId);
        await _testService.SaveAnswerSimpleTest(testId, userAnswers, userId);
        return Ok();
    }
    
    /// <summary>
    /// Answer the simple test 
    /// </summary>
    [HttpPost]
    [Route("{testId}/simple")]
    public async Task<ActionResult> AnswerSimpleTest(Guid testId, [FromBody] UserAnswerSimpleDto userAnswers) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentTestPermission(userId, testId);
        await _testService.SaveAnswerSimpleTest(testId, userAnswers, userId);
        await _testService.AnswerSimpleTest(testId, userId);
        return Ok();
    }

    /// <summary>
    /// Add answer to simple test
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> AddAnswerToSimpleTest(Guid testId, [FromBody] SimpleAnswerDto newAnswer)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.AddAnswerToSimpleTest(testId, newAnswer);
        return Ok();
    }

    /// <summary>
    /// Edit answer in simple test
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> EditAnswerInSimpleTest(Guid testId, Guid answerId, [FromBody] SimpleAnswerDto answer)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.EditAnswerInSimpleTest(answerId, answer);
        return Ok();
    }

    /// <summary>
    /// Delete answer from simple test
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> DeleteAnswerFromSimpleTest(Guid testId, Guid answerId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.DeleteAnswerFromSimpleTest(testId, answerId);
        return Ok();
    }


    /// <summary>
    /// Add correct sequence test to chapter
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> AddCorrectSequenceTestToChapter(Guid chapterId, [FromBody] TestCorrectSequenceCreateDto model) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorChapterPermission(chapterId, userId);
        await _testService.AddCorrectSequenceTestToChapter(chapterId, model);
        return Ok();
    }

    /// <summary>
    /// Save answer for correct sequence test type
    /// </summary>
    [HttpPut]
    [Route("{testId}/correct-sequence/save")]
    public async Task<ActionResult> SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentTestPermission(userId, testId);
        await _testService.SaveAnswerCorrectSequenceTest(testId, userAnswers, userId);
        return Ok();
    }

    /// <summary>
    /// Answer the correct sequence test 
    /// </summary>
    [HttpPost]
    [Route("{testId}/correct-sequence")]
    public async Task<ActionResult> AnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentTestPermission(userId, testId);
        await _testService.SaveAnswerCorrectSequenceTest(testId, userAnswers, userId);
        await _testService.AnswerCorrectSequenceTest(testId, userId);
        return Ok();
    }

    /// <summary>
    /// Add answer to sequence test
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> AddAnswerToSequenceTest(Guid testId, [FromBody] CorrectSequenceAnswerDto newAnswerModel)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.AddAnswerToSequenceTest(testId, newAnswerModel);
        return Ok();
    }

    /// <summary>
    /// Edit answer in sequence test
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> EditAnswerInSequenceTest(Guid testId, [FromBody] CorrectSequenceAnswerDto answerModel)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.AddAnswerToSequenceTest(testId, answerModel);
        return Ok();
    }

    /// <summary>
    /// Delete answer from sequence test
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> DeleteAnswerFromSequenceTest(Guid testId, Guid answerId)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.DeleteAnswerFromSequenceTest(testId, answerId);
        return Ok();
    }

    /// <summary>
    /// Add detailed test to chapter
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("chapter/{chapterId}/detailed")]
    public async Task<ActionResult> AddDetailedTestToChapter(Guid chapterId, [FromBody] TestDetailedCreateDto testModel)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorChapterPermission(chapterId, userId);
        await _testService.AddDetailedTestToChapter(chapterId, testModel);
        return Ok();
    }

    /// <summary>
    /// Answer the detailed test 
    /// </summary>
    [HttpPost]
    [Route("{testId}/detailed")]
    public async Task<ActionResult> AnswerDetailedTest(Guid testId, [FromBody] DetailedAnswerDto userAnswer)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentTestPermission(userId, testId);
        await _testService.SaveAnswerDetailedTest(testId, userAnswer, userId);
        await _testService.AnswerDetailedTest(testId, userId);
        return Ok();
    }

    /// <summary>
    /// Save answer for detailed test
    /// </summary>
    [HttpPut]
    [Route("{testId}/detailed/save")]
    public async Task<ActionResult> SaveAnswerDetailedTest(Guid testId, [FromBody] DetailedAnswerDto userAnswer)
    {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckStudentTestPermission(userId, testId);
        await _testService.SaveAnswerDetailedTest(testId, userAnswer, userId);
        return Ok();
    }

    /// <summary>
    /// Archive test 
    /// </summary>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher + "," + ApplicationRoleNames.Administrator)]
    [Route("{testId}")]
    public async Task<ActionResult> ArchiveTest(Guid testId) {
        if (User.Identity == null || Guid.TryParse(User.Identity.Name, out Guid userId) == false)
        {
            throw new UnauthorizedException("User is not authorized");
        }

        await _checkPermissionService.CheckCreatorTestPermission(userId, testId);
        await _testService.ArchiveTest(testId);
        return Ok();
    }
}