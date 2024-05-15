using LeoPasswordManager.Contexts;
using LeoPasswordManager.Models;

namespace LeoPasswordManager.Interfaces;

public interface IAccountRepository
{
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<AuthStatus> ChangePasswordAsync(string userId, string newPassword);
    Task<AuthStatus>  CheckPassword(string email, string password);
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
