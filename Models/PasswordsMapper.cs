using MVC_RazorComp_PasswordManager.Contexts;
using CsvHelper.Configuration;

namespace MVC_RazorComp_PasswordManager.Models;

public class PasswordsMapper : ClassMap<PasswordmanagerAccount>
{
    public PasswordsMapper()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Username).Name("Username");
        Map(m => m.Password).Name("Password");
    }
}