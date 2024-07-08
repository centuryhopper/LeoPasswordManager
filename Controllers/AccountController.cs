using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeoPasswordManager.Models;
using LeoPasswordManager.Utilities;
using Microsoft.AspNetCore.Identity;
using LeoPasswordManager.Contexts;
using LeoPasswordManager.Interfaces;


namespace LeoPasswordManager.Controllers;


[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> logger;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IConfiguration configuration;
    private readonly IHostEnvironment env;
    private readonly IPasswordManagerDbRepository<PasswordAccountModel> passwordManagerDbRepository;

    public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IHostEnvironment env, IPasswordManagerDbRepository<PasswordAccountModel> passwordManagerDbRepository)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        this.configuration = configuration;
        this.env = env;
        this.passwordManagerDbRepository = passwordManagerDbRepository;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginVM vm, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(
                vm.Email, vm.Password, vm.RememberMe, false);

            if (result.Succeeded)
            {
                var umsUser = await userManager.GetUserAsync(User);
                var markUserLogout = await passwordManagerDbRepository.MarkUserLoggedIn(umsUser!.Id);
                return RedirectToAction(nameof(HomeController.Welcome), "Home");
            }

            // ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            TempData[TempDataKeys.ALERT_ERROR] = "Invalid login attempt!";
        }

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return View(vm);
    }

    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        var umsUser = await userManager.GetUserAsync(User);
        var markUserLogout = await passwordManagerDbRepository.MarkUserLoggedOut(umsUser!.Id);
        return RedirectToAction(nameof(HomeController.Welcome), "Home");
    }

    // public async Task<IActionResult> EditProfile()
    // {
    //     ViewBag.UserId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
    //     return View();
    // }
}
