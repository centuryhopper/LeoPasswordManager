using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Server.Repositories;
using static Shared.Models.ServiceResponses;
using Newtonsoft.Json;

namespace Server.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PasswordManagerController(ILogger<PasswordManagerController> logger, IPasswordManagerDbRepository passwordManagerAccountRepository) : ControllerBase
{
    [HttpDelete("deletePassword/{passwordRecordId:int}")]
    public async Task<IActionResult> DeletePasswordRecord(int passwordRecordId)
    {
        var response = await passwordManagerAccountRepository.DeleteAsync(passwordRecordId);
        if (response is null)
        {
            return NotFound("This record doesn't exist.");
        }

        if (!response.Flag)
        {
            return BadRequest(response.Message);
        }
        return Ok(response.Message);
    }

    [HttpGet("getPassword/{passwordRecordId:int}")]
    public async Task<IActionResult> GetPasswordRecord(int passwordRecordId)
    {
        var passwordRecord = await passwordManagerAccountRepository.GetPasswordRecordAsync(passwordRecordId);
        if (passwordRecord is null)
        {
            return NotFound("This record doesn't exist.");
        }
        return Ok(passwordRecord);
    }

    [HttpGet("passwords")]
    public async Task<IActionResult> GetAllPasswordRecords(int pg = 1)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        logger.LogWarning(userId);
        var passwordAccounts = await passwordManagerAccountRepository.GetAllPasswordRecordsAsync(Convert.ToInt32(userId));
        return Ok(passwordAccounts);
    }

    [HttpPost("upload-csv")]
    public async Task<IActionResult> UploadCSV(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "File is missing or empty." });
        }

        // Add debug logs or console output
        // Console.WriteLine($"File Name: {file.FileName}");
        // Console.WriteLine($"File Length: {file.Length}");
        // Console.WriteLine($"User ID: {userId}");
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var response = await passwordManagerAccountRepository.UploadCsvAsync(file, userId);

        // logger.LogWarning(result.Message);
        // System.Console.WriteLine("nooo");

        if (!response.Flag)
        {
            return BadRequest(response.Message);
        }
        return Ok(response.Message);
    }

    [HttpPost("add-passwords")]
    public async Task<IActionResult> AddPasswordRecords([FromBody] string values)
    {
        IEnumerable<PasswordAccountDTO> passwordAccountDTOs = JsonConvert.DeserializeObject<IEnumerable<PasswordAccountDTO>>(values);
        var response = await passwordManagerAccountRepository.CreateMultipleAsync(passwordAccountDTOs);
        if (!response.Flag)
        {
            return BadRequest(new GeneralResponse(Flag: false, Message: response.Message));
        }
        return Ok(new GeneralResponse(Flag: true, Message: response.Message));
    }

    [HttpPost("add-password")]
    public async Task<IActionResult> AddPasswordRecord([FromBody] PasswordAccountDTO passwordAccountDTO)
    {
        var response = await passwordManagerAccountRepository.CreateAsync(passwordAccountDTO);
        if (!response.Flag)
        {
            return BadRequest(response.Message);
        }
        return Ok(response.Message);
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePasswordRecord([FromBody] PasswordAccountDTO passwordAccountDTO)
    {
        var response = await passwordManagerAccountRepository.UpdateAsync(passwordAccountDTO);
        if (!response.Flag)
        {
            return BadRequest(response.Message);
        }
        return Ok(response.Message);
    }
}


