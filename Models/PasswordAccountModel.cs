namespace LeoPasswordManager.Models;

public class PasswordAccountModel
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? Title { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? CreatedAt { get; set; }
    public string? LastUpdatedAt { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}:{Id}, {nameof(Title)}:{Title}, {nameof(Username)}:{Username}, {nameof(Password)}:{Password}, {nameof(UserId)}:{UserId}";
    }
}