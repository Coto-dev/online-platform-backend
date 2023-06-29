using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using HW.Account.DAL.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using HW.EmailReciever.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace HW.EmailReciever.MVC.Controllers;

public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration) {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index() {
        return View();
    }

    public IActionResult Privacy() {
        return View();
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null) {
            return View("Error");
        }

        var client = new HttpClient();
        var encode = HttpUtility.UrlEncode(code);
        var config = _configuration.GetSection("ConfirmUrl");
        var result = await client.GetAsync(config.GetValue<string>("Url")+$"?code={encode}&userId={userId}");
        if(result.StatusCode == HttpStatusCode.OK)
            return RedirectToAction("Privacy", "Home");
        else
            return View("Error");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}