using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeoPasswordManager.Interfaces;
using LeoPasswordManager.Models;
using LeoPasswordManager.Utilities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using LeoPasswordManager.Entities;

namespace LeoPasswordManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;
    private readonly IPasswordManagerDbRepository<PasswordAccountModel> passwordManagerAccountRepository;
    private readonly UserManager<ApplicationUser> userManager;

    public HomeController(ILogger<HomeController> logger, IPasswordManagerDbRepository<PasswordAccountModel> passwordManagerAccountRepository, UserManager<ApplicationUser> userManager)
    {
        this.logger = logger;
        this.passwordManagerAccountRepository = passwordManagerAccountRepository;
        this.userManager = userManager;
    }

    public IActionResult Welcome()
    {
        return View();
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> UploadCSV(IFormFile file, int userId)
    {
        var result = await passwordManagerAccountRepository.UploadCsvAsync(file, userId);

        if (result.UploadEnum == UploadEnum.FAIL)
        {
            return BadRequest("failed to upload csv");
        }

        return Ok("upload csv success!");
        // return RedirectToAction(nameof(Index), new {pg=1});

    }



    [HttpPost]
    public async Task<IActionResult> UpdatePasswordDetail(PasswordAccountModel passwordAccountModel)
    {
        List<string> errors = new();
        if (!ModelState.IsValid)
        {
            errors = ModelState.GetErrors<HomeController>().ToList();
            TempData[TempDataKeys.ALERT_ERROR] = string.Join("$$$", errors);
            return RedirectToAction(nameof(Index));
        }

        var updatedPasswordDetail = await passwordManagerAccountRepository.UpdateAsync(passwordAccountModel);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeletePasswordDetail(string passwordDetailId)
    {
        var umsUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        // get user id from finding user via ums userid
        var user = await passwordManagerAccountRepository.GetPasswordManagerUser(umsUserId: umsUserId);
        var accountModel = await passwordManagerAccountRepository.GetAccountModelAsync(passwordDetailId, user!.Id);
        var deleteDetail = await passwordManagerAccountRepository.DeleteAsync(accountModel!);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddPasswordRows([FromBody] dynamic obj)
    {
        List<string> errors = new();
        if (!ModelState.IsValid)
        {
            errors = ModelState.GetErrors<HomeController>().ToList();
            TempData[TempDataKeys.ALERT_ERROR] = string.Join("$$$", errors);
            return RedirectToAction(nameof(Index));
        }

        // logger.LogWarning($"{obj.ToString()}");

        AddPasswordRowsVM model = JsonConvert.DeserializeObject<AddPasswordRowsVM>(obj.ToString())!;

        foreach (var acc in model.PasswordAccountModels)
        {
            // logger.LogWarning($"{acc.Title}");
            // logger.LogWarning($"{acc.Username}");
            // logger.LogWarning($"{acc.Password}");

            await passwordManagerAccountRepository.CreateAsync(acc);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Index(int pg = 1)
    {
        logger.LogWarning("logged in!");
        // var umsUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var umsUser = await userManager.GetUserAsync(User);
        

        // get user id from finding user via ums userid
        var user = (await passwordManagerAccountRepository.GetPasswordManagerUser(umsUserId: umsUser!.Id)) ?? await passwordManagerAccountRepository.CreatePasswordManagerUser(umsUser);

        ViewBag.userId = user!.Id;

        const int PAGE_SIZE = 5;
        if (pg < 1) pg = 1;
        var passwordAccounts = await passwordManagerAccountRepository.GetAllAccountsAsync(user!.Id);
        var pager = new Pager(totalItems: passwordAccounts.Count(), pageNumber: pg, pageSize: PAGE_SIZE);
        int recordsToSkip = (pg - 1) * PAGE_SIZE;
        var pagedDetails = passwordAccounts.Skip(recordsToSkip).Take(pager.PageSize);

        ViewBag.PagedDetails = pagedDetails;
        ViewBag.Pager = pager;
        ViewBag.TotalRecordsCount = passwordAccounts.Count();

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
