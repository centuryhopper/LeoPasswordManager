using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_RazorComp_PasswordManager.Contexts;
using MVC_RazorComp_PasswordManager.Interfaces;

namespace MVC_RazorComp_PasswordManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPasswordManagerAccountRepository<PasswordmanagerAccount> passwordManagerAccountRepository;

    public HomeController(ILogger<HomeController> logger, IPasswordManagerAccountRepository<PasswordmanagerAccount> passwordManagerAccountRepository)
    {
        _logger = logger;
        this.passwordManagerAccountRepository = passwordManagerAccountRepository;
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> UploadCSV(IFormFile file, string userId)
    {
        
        var result = await passwordManagerAccountRepository.UploadCsvAsync(file, userId);

        if (result is null)
        {
            return BadRequest("failed to upload csv");
        }

        return Ok("upload csv success!");
    }

    [Authorize]
    public IActionResult Index()
    {
        var userId = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        ViewBag.userId = userId;
        return View();
    }


    [Authorize]
    public IActionResult Settings()
    {
        return View();
    }
    

    // public IActionResult Privacy()
    // {
    //     return View();
    // }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
