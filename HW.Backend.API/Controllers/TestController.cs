using HW.Common.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for test management
/// </summary>
[ApiController]
[Route("api/test")]
public class TestController : ControllerBase {
    private readonly ILogger<TestController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public TestController(ILogger<TestController> logger) {
        _logger = logger;
    }
    
    /// <summary>
    /// Add simple test to chapter
    /// </summary>
    [HttpPost]
    [Route("chapter/{chapterId}/simple")]
    public async Task<ActionResult> AddTestToChapter(Guid chapterId, [FromBody] TestSimpleCreateDto model) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Edit simple test 
    /// </summary>
    [HttpPut] 
    [Route("{testId}/simple")]
    public async Task<ActionResult> EditSimpleTest(Guid testId, [FromBody] TestSimpleCreateDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Save answer for simple test type
    /// </summary>
    [HttpPut]
    [Route("{testId}/simple/save")]
    public async Task<ActionResult> SaveAnswerType1(Guid testId, [FromBody] List<UserAnswerSimpleDto> userAnswers) {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
    public async Task<ActionResult> SaveAnswerType2(Guid testId, List<UserAnswerCorrectSequenceDto> userAnswers) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Archive test 
    /// </summary>
    [HttpDelete] 
    [Route("{testId}")]
    public async Task<ActionResult> ArchiveTest(Guid testId) {
        throw new NotImplementedException();
    }
}