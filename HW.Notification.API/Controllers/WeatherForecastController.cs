using System.Web;
using HW.Common.Exceptions;
using HW.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HW.Notification.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase {
    private readonly IAuthService _authService;

    public WeatherForecastController(IAuthService authService) {
        _authService = authService;
    }


}