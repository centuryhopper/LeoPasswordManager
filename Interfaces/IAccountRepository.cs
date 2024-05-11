using MVC_RazorComp_PasswordManager.Contexts;
using MVC_RazorComp_PasswordManager.Models;

namespace MVC_RazorComp_PasswordManager.Interfaces;

public interface IAccountRepository
{
    Task<AuthStatus> LoginAsync(LoginModel model);
    Task<AuthStatus> LogoutAsync(string userId);
    Task<AuthStatus> RegisterAsync(RegisterModel model);
    Task<PasswordmanagerUser?> GetUserByIdAsync(string UserId);
    Task<IEnumerable<string>> GetRolesAsync();
    Task<IEnumerable<string>> UpdateUserAsync(EditAccountModel model);
    Task<DeleteUserProfileStatus> DeleteUserAsync(string Id);
    Task<bool> VerifyTokenAsync(AccountProviders accountProviders, string token, string userId);
    Task<EmailConfirmStatus> IsEmailConfirmed(string email);
}
