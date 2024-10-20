using Microsoft.AspNetCore.Http;
using Server.Contexts;
using Server.Entities;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public interface IPasswordManagerDbRepository
{
    Task<PasswordAccountDTO?> GetPasswordRecordAsync(int passwordRecordId);
    Task<IEnumerable<PasswordAccountDTO>> GetAllPasswordRecordsAsync(int userId);
    Task<GeneralResponse> CreateAsync(PasswordAccountDTO model);
    Task<GeneralResponse> CreateMultipleAsync(IEnumerable<PasswordAccountDTO> passwordsToAdd);
    Task<GeneralResponse> UpdateAsync(PasswordAccountDTO model);
    Task<GeneralResponse> DeleteAsync(int passwordRecordId);
    Task<GeneralResponse> UploadCsvAsync(IFormFile file, int userId);
}


