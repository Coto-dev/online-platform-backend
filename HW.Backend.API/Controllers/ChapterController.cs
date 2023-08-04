using HW.Common.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers; 

/// <summary>
/// Controller for module management
/// </summary>
[ApiController]
[Route("api/chapter")]
public class ChapterController : ControllerBase {
    private readonly ILogger<ChapterController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public ChapterController(ILogger<ChapterController> logger) {
        _logger = logger;
    }
    
    /// <summary>
    /// Get chapter by id
    /// </summary>
    [HttpGet]
    [Route("{chapterId}")]
    public async Task<ActionResult<ChapterFullDto>> GetChapter(Guid chapterId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Make chapter learned
    /// </summary>
    [HttpPost]
    [Route("{chapterId}")]
    public async Task<ActionResult> LearnChapter(Guid chapterId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Send comment
    /// </summary>
    [HttpPost]
    [Route("{chapterId}/comment")]
    public async Task<ActionResult> SendComment([FromBody] ChapterCommentSendDto message, Guid chapterId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Delete comment
    /// </summary>
    [HttpDelete]
    [Route("comment/{commentId}")]
    public async Task<ActionResult> DeleteComment(Guid commentId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Edit comment
    /// </summary>
    [HttpPut]
    [Route("comment/{commentId}")]
    public async Task<ActionResult> EditComment(Guid commentId) {
        throw new NotImplementedException();
    }
}