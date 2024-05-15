using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeoPasswordManager.Interfaces;
using LeoPasswordManager.Models;
using LeoPasswordManager.Utilities;

namespace LeoPasswordManager.Controllers;

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
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (ModelState.IsValid)
        {
            var user = await accountRepository.GetUserByEmailAsync(model.CurrentEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // The new password did not meet the complexity rules or
            // the current password is incorrect. Add these errors to
            // the ModelState and rerender ChangePassword view

            var authStatus = await accountRepository.CheckPassword(user.Email, model.CurrentPassword);
            if (!authStatus.Successful)
            {
                ModelState.AddModelError(string.Empty, authStatus.Error!);
                return View();
            }

            await accountRepository.ChangePasswordAsync(user.Id, model.NewPassword);

            return View("ChangePasswordConfirmation");
        }

        return View(model);
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
