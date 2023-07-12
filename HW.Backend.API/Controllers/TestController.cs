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
    /// Answer the test 
    /// </summary>
    [HttpPost]
    [Route("{testId}")]
    public async Task<ActionResult> AnswerTest(Guid testId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Save answer for test1 type
    /// </summary>
    [HttpPut]
    [Route("{testId}/type1")]
    public async Task<ActionResult> SaveAnswerType1(Guid testId) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Save answer for test2 type
    /// </summary>
    [HttpPut]
    [Route("{testId}/type2")]
    public async Task<ActionResult> SaveAnswerType2(Guid testId) {
        throw new NotImplementedException();
    }
    
}