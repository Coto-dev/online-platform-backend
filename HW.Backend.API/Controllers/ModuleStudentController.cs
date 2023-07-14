using HW.Common.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for module management
/// </summary>
[ApiController]
[Route("api/module")]
public class ModuleStudentController : ControllerBase {
   

    private readonly ILogger<ModuleStudentController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public ModuleStudentController(ILogger<ModuleStudentController> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Get all available modules
    /// </summary>
    [HttpGet]
    [Route("all/list")]
    public async Task<ActionResult<ModuleShortDto>> GetAvailableModules() {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get purchased modules
    /// </summary>
    [HttpGet]
    [Route("purchased/list")]
    public async Task<ActionResult<ModuleShortDto>> GetPurchasedModules() {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get module content by moduleId
    /// </summary>
    ///<remarks>
    /// return module with submodules and first chapter
    /// </remarks>
    [HttpGet]
    [Route("{moduleId}/content")]
    public async Task<ActionResult<ModuleFullDto>> GetModuleContent(Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get chapter content by chapterId
    /// </summary>
    ///<remarks>
    /// return module with submodules and first chapter
    /// </remarks>
    [HttpGet]
    [Route("chapter/{chapterId}")]
    public async Task<ActionResult<ChapterFullDto>> GetChapterContent(Guid chapterId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Get module details by moduleId
    /// </summary>
    [HttpGet]
    [Route("{moduleId}/details")]
    public async Task<ActionResult<ModuleDetailsDto>> GetModuleDetails(Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Send comment to module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/comment")]
    public async Task<ActionResult<ModuleDetailsDto>> SendCommentToModule([FromBody] ModuleCommentDto model, Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Buy module
    /// </summary>
    [HttpPost]
    [Route("{moduleId}")]
    public async Task<ActionResult<ModuleDetailsDto>> BuyModule(Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Add module to basket
    /// </summary>
    [HttpPost]
    [Route("{moduleId}/basket")]
    public async Task<ActionResult> AddModuleToBasket(Guid moduleId) {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Delete module from basket
    /// </summary>
    [HttpDelete]
    [Route("{moduleId}/basket")]
    public async Task<ActionResult> DeleteModuleToBasket(Guid moduleId) {
        throw new NotImplementedException();
    }
}