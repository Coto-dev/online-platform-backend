using HW.Backend.DAL.Data.Entities;
using HW.Common.DataTransferObjects;
using HW.Common.Enums;
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
    [Route("chapter/{chapterId}/simple")]
    //[Authorize(AuthenticationSchemes = "Bearer", Roles = ApplicationRoleNames.Teacher)]
    public async Task<ActionResult> AddSimpleTestToChapter(Guid chapterId, [FromBody] TestSimpleCreateDto testModel) {

        await _testService.AddSimpleTestToChapter(chapterId, testModel);
        return Ok();
    }

    /// <summary>
    /// Edit simple test 
    /// </summary>
    [HttpPut] 
    [Route("{testId}/simple")]
    public async Task<ActionResult> EditSimpleTest(Guid testId, [FromBody] TestSimpleCreateDto testModel) {

        await _testService.EditSimpleTest(testId, testModel);
        return Ok();
    }
    
    /// <summary>
    /// Save answer for simple test type
    /// </summary>
    [HttpPut]
    [Route("{testId}/simple/save")]
    public async Task<ActionResult> SaveAnswerSimpleTest(Guid testId, [FromBody] List<UserAnswerSimpleDto> userAnswers) {


        await _testService.SaveAnswerSimpleTest(testId, userAnswers);
        return Ok();
    }
    
    /// <summary>
    /// Answer the simple test 
    /// </summary>
    [HttpPost]
    [Route("{testId}/simple")]
    public async Task<ActionResult> AnswerSimpleTest(Guid testId, [FromBody] List<UserAnswerSimpleDto> userAnswers) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Add correct sequence test to chapter
    /// </summary>
    [HttpPost]
    [Route("chapter/{chapterId}/correct-sequence")]
    public async Task<ActionResult> AddCorrectSequenceTestToChapter(Guid chapterId, [FromBody] TestCorrectSequenceCreateDto model) {

        await _testService.AddCorrectSequenceTestToChapter(chapterId, model);
        return Ok();
    }
    
    /// <summary>
    /// Edit correct sequence test(save changes)
    /// </summary>
    [HttpPut]
    [Route("{testId}/correct-sequence")]
    public async Task<ActionResult> EditCorrectSequenceTest(Guid testId, [FromBody] TestCorrectSequenceCreateDto model) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Answer the correct sequence test 
    /// </summary>
    [HttpPost]
    [Route("{testId}/correct-sequence")]
    public async Task<ActionResult> AnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers) {
        throw new NotImplementedException();
    }
    
    
    /// <summary>
    /// Save answer for correct sequence test type
    /// </summary>
    [HttpPut]
    [Route("{testId}/correct-sequence/save")]
    public async Task<ActionResult> SaveAnswerCorrectSequenceTest(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add detailed test to chapter
    /// </summary>
    [HttpPost]
    [Route("chapter/{chapterId}/detailed")]
    public async Task<ActionResult> AddDetailedTestToChapter(Guid chapterId)
    {

        throw new NotImplementedException();
    }

    /// <summary>
    /// Edit detailed test(save changes)
    /// </summary>
    [HttpPut]
    [Route("{testId}/detailed")]
    public async Task<ActionResult> EditDetailedTest(Guid testId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Answer the detailed test 
    /// </summary>
    [HttpPost]
    [Route("{testId}/detailed")]
    public async Task<ActionResult> AnswerDetailedTest(Guid testId)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Save answer for detailed test
    /// </summary>
    [HttpPut]
    [Route("{testId}/detailed/save")]
    public async Task<ActionResult> SaveAnswerDetailedTest(Guid testId)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Archive test 
    /// </summary>
    [HttpDelete] 
    [Route("{testId}")]
    public async Task<ActionResult> ArchiveTest(Guid testId) {

        await _testService.ArchiveTest(testId);
        return Ok();
    }
}