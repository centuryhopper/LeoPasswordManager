

using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public interface IAccountRepository
{
    Task<GeneralResponse> Logout(int userId);
    Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
    Task<GeneralResponse> CheckPassword(string email, string password);
}
