using System.Drawing;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Repositories;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger) : ControllerBase
{
    [HttpGet("nlog-test")]
    public IActionResult Test()
    {
        logger.LogWarning("warning!");
        logger.LogInformation("information!");
        logger.LogError("error!");
        logger.LogCritical("critical");
        logger.LogDebug("debug");
        return Ok("Logging test completed. Check your PostgreSQL LOGS table.");
    }

    [Authorize]
    [HttpGet("check-password")]
    public async Task<IActionResult> CheckPassword([FromQuery] string password)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var response = await accountRepository.CheckPassword(email!, password);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout/{userId:int}")]
    public async Task<IActionResult> Logout(int userId)
    {
        var response = await accountRepository.Logout(userId);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        //logger.LogInformation(response.Message);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        var response = await accountRepository.LoginAccount(loginDTO);
        if (!response.Flag)
        {
            return BadRequest(response);
        }
        //logger.LogInformation(response.Message);
        return Ok(response);
    }
}
