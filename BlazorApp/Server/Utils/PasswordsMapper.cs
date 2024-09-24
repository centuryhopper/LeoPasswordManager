using CsvHelper.Configuration;
using Server.Models;

namespace Server.Utils;

public class PasswordsMapper : ClassMap<PasswordAccountDTO>
{
    public PasswordsMapper()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Username).Name("Username");
        Map(m => m.Password).Name("Password");
    }
}