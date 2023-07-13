using Microsoft.AspNetCore.Mvc;

namespace HW.Backend.API.Controllers;

/// <summary>
/// Controller for admin
/// </summary>
[ApiController]
[Route("api/admin-panel")]
public class AdminPanelController : ControllerBase {
    
    private readonly ILogger<AdminPanelController> _logger;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public AdminPanelController(ILogger<AdminPanelController> logger) {
        _logger = logger;
    }
    
}