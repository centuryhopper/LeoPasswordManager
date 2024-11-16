namespace Shared.Models;

using System.Collections.Generic;
using CsvHelper.Configuration;


public class PasswordAccountDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}:{Id}, {nameof(Title)}:{Title}, {nameof(Username)}:{Username}, {nameof(Password)}:{Password}, {nameof(UserId)}:{UserId}";
    }
}


public class PasswordsMapper : ClassMap<PasswordAccountDTO>
{
    public PasswordsMapper()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Username).Name("Username");
        Map(m => m.Password).Name("Password");
    }
}