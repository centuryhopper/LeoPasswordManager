

using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public interface IAccountRepository
{
    Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
}
