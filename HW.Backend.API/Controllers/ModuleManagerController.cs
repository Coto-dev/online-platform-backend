using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers; 

/// <summary>
/// Controller for teacher module management
/// </summary>
[ApiController]
[Route("api/module")]
public class ModuleManagerController : ControllerBase {
   

    private readonly ILogger<ModuleManagerController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public ModuleManagerController(ILogger<ModuleManagerController> logger) {
        _logger = logger;
    }
    
    /// <summary>
    /// Get my created modules
    /// </summary>
    [HttpGet]
    [Route("my/list")]
    public async Task<ActionResult<ModuleShortDto>> GetMyModules([FromQuery] ModuleStatusType? filter) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Create self-study module
    /// </summary>
    [HttpPost]
    [Route("self-study")]
    public async Task<ActionResult> CreateSelfStudyModule([FromBody] ModuleSelfStudyCreateDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Edit self-study module
    /// </summary>
    [HttpPut]
    [Route("self-study")]
    public async Task<ActionResult> EditSelfStudyModule([FromBody] ModuleSelfStudyEditDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Create streaming module
    /// </summary>
    [HttpPost]
    [Route("streaming")]
    public async Task<ActionResult> CreateStreamingModule([FromBody] ModuleStreamingCreateDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Edit streaming module
    /// </summary>
    [HttpPut]
    [Route("streaming")]
    public async Task<ActionResult> EditStreamingModule([FromBody] ModuleStreamingEditDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Archive my module
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}")]
    public async Task<ActionResult> ArchiveModule(Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Add sub module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/sub-module")]
    public async Task<ActionResult> AddSubModule(Guid moduleId, [FromBody] SubModuleCreateDto model) {
        throw new NotImplementedException();
    }
    //TODO: создание chapter, просмотр саб моулей и глав в режиме редактирования.
    /// <summary>
    /// Add chapter to sub module
    /// </summary>
    [HttpPost]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> AddChapter(Guid subModuleId) {
        throw new NotImplementedException();
    }
    
}