using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_RazorComp_PasswordManager.Interfaces;
using MVC_RazorComp_PasswordManager.Models;
using MVC_RazorComp_PasswordManager.Utilities;

namespace MVC_RazorComp_PasswordManager.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountRepository accountRepository;

    public AccountController(ILogger<AccountController> logger, IAccountRepository accountRepository)
    {
        _logger = logger;
        this.accountRepository = accountRepository;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string token, string userId)
    {
        var verifyToken = await accountRepository.VerifyTokenAsync(AccountProviders.EMAIL_CONFIRMATION, token, userId);

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel vm, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            var lst = Helpers.GetErrors<AccountController>(ModelState).ToList();
            TempData[TempDataKeys.ALERT_ERROR] = string.Join("$$$", lst);
            return RedirectToAction(nameof(Login));
        }

        if (await accountRepository.IsEmailConfirmed(vm.Email) == EmailConfirmStatus.NOT_CONFIRMED)
        {
            TempData[TempDataKeys.ALERT_ERROR] = "Please confirm your email.";
            return RedirectToAction(nameof(Login));
        }

        // we don't tell the user that this email isn't in our database to avoid data breaching
        // that's why we give a generic error message below
        if (await accountRepository.IsEmailConfirmed(vm.Email) == EmailConfirmStatus.ACCOUNT_NOT_REGISTERED)
        {
            TempData[TempDataKeys.ALERT_ERROR] = "Your identity couldn't be verified with us.";
            return RedirectToAction(nameof(Login));
        }

        var result = await accountRepository.LoginAsync(vm);


        if (result.Successful)
        {
            List<Claim> claims = [
                new(ClaimTypes.NameIdentifier, result.Id),
                new(ClaimTypes.Email, result.Email),
                new(ClaimTypes.Role, result.Role),
            ];

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction(nameof(HomeController.Index), "Home", new { userId = result.Id });
        }
        else
        {
            TempData[TempDataKeys.ALERT_ERROR] = result.Error;
        }

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel vm, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            var lst = Helpers.GetErrors<AccountController>(ModelState).ToList();
            TempData[TempDataKeys.ALERT_ERROR] = string.Join("$$$", lst);
            return RedirectToAction(nameof(Register));
        }
        
        var result = await accountRepository.RegisterAsync(vm);

        if (result.Successful)
        {
            TempData[TempDataKeys.ALERT_SUCCESS] = "Registration Successful! Please confirm your email to get started.";
        }
        else
        {
            TempData[TempDataKeys.ALERT_ERROR] = result.Error;
        }

        return RedirectToAction(nameof(Register));
    }

    public async Task<IActionResult> EditProfile()
    {
        ViewBag.UserId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        return View();
    }

    public async Task<IActionResult> LogOut()
    {
        var userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var logout = await accountRepository.LogoutAsync(userId);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }

}
