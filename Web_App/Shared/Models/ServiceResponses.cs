
namespace Shared.Models;

public class ServiceResponses
{
    public record class GridValidationResponse(bool Flag, Dictionary<int, List<string>>? ErrorMessagesDict);
    public record class GeneralResponse(bool Flag, string Message);
    public record class LoginResponse(bool Flag, string Token, string Message);
}