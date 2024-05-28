using Microsoft.AspNetCore.Http;
using LeoPasswordManager.Models;

namespace LeoPasswordManager.Interfaces;

public interface IPasswordManagerAccountRepository<T>
{
    Task<PasswordAccountModel?> GetAccountModelAsync(string id, string userId);
    Task<IEnumerable<T>> GetAllAccountsAsync(string userId);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}