using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Repositories;

namespace Server.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PasswordManagerController(ILogger<PasswordManagerController> logger, IPasswordManagerDbRepository passwordManagerAccountRepository) : ControllerBase
{
    [HttpDelete("deletePassword/{passwordRecordId:int}")]
    public async Task<IActionResult> DeletePasswordRecordAsync(int passwordRecordId)
    {
        var response = await passwordManagerAccountRepository.DeleteAsync(passwordRecordId);
        if (!response.Flag)
        {
            return BadRequest(response.Message);
        }
        return Ok("");
    }

    [Authorize]
    [HttpGet("passwords")]
    public async Task<IActionResult> Passwords(int pg = 1)
    {
        var passwordAccounts = await passwordManagerAccountRepository.GetAllAccountsAsync(user!.Id);
        return Ok(passwordAccounts);
    }

    [Authorize, HttpPost("upload-csv")]
    public async Task<IActionResult> UploadCSV(IFormFile file, int userId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "File is missing or empty." });
        }

        // Add debug logs or console output
        // Console.WriteLine($"File Name: {file.FileName}");
        // Console.WriteLine($"File Length: {file.Length}");
        // Console.WriteLine($"User ID: {userId}");

        var result = await passwordManagerAccountRepository.UploadCsvAsync(file, userId);

        // logger.LogWarning(result.Message);
        // System.Console.WriteLine("nooo");

        if (result.UploadEnum == UploadEnum.FAIL)
        {
            return BadRequest(new { message = result.Message });
        }

        return Json(new { message = result.Message });
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePasswordDetail([FromBody] PasswordAccountDTO passwordAccountDTO)
    {
        List<string> errors = new();
        if (!ModelState.IsValid)
        {
            errors = ModelState.GetErrors<HomeController>().ToList();
            TempData[TempDataKeys.ALERT_ERROR] = string.Join("$$$", errors);
            return RedirectToAction(nameof(Passwords));
        }

        var updatedPasswordDetail = await passwordManagerAccountRepository.UpdateAsync(passwordAccountDTO);

        return RedirectToAction(nameof(Passwords));
    }
}


