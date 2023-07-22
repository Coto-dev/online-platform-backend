using HW.Common.DataTransferObjects;
using HW.Common.Enums;
using HW.Common.Other;
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
    /// Get teacher's created modules
    /// </summary>
    [HttpGet]
    [Route("teacher/list")]
    public async Task<ActionResult<PagedList<ModuleShortDto>>> GetMyModules([FromQuery] PaginationParamsDto pagination, 
        [FromQuery] ModuleFilterTeacherType? section = ModuleFilterTeacherType.Published) {
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
    /// Create sub module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/sub-module")]
    public async Task<ActionResult> AddSubModule(Guid moduleId, [FromBody] SubModuleCreateDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Edit sub module
    /// </summary>
    [HttpPut]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> EditSubModule(Guid subModuleId, [FromBody] SubModuleEditDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Archive sub module
    /// </summary>
    [HttpDelete]
    [Route("sub-module/{subModuleId}")]
    public async Task<ActionResult> ArchiveSubModule(Guid subModuleId) {
        throw new NotImplementedException();
    }
    
    //TODO: создание chapter, просмотр саб моулей и глав в режиме редактирования.
    /// <summary>
    /// Create chapter in sub module
    /// </summary>
    [HttpPost]
    [Route("sub-module/{subModuleId}/chapter")]
    public async Task<ActionResult> CreateChapter(Guid subModuleId, [FromBody] ChapterCreateDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Edit chapter(save changes)
    /// </summary>
    [HttpPut]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> EditChapter(Guid chapterId, [FromBody] ChapterEditDto model) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Archive chapter
    /// </summary>
    [HttpDelete]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult> ArchiveChapter(Guid chapterId) {
        throw new NotImplementedException();
    }
    
}