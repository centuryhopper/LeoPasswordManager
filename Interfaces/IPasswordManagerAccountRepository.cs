using Microsoft.AspNetCore.Http;
using MVC_RazorComp_PasswordManager.Models;

namespace MVC_RazorComp_PasswordManager.Interfaces;

public interface IPasswordManagerAccountRepository<T>
{
    Task<IEnumerable<T>> GetAllAccountsAsync(string userId);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}