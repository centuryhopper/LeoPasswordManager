using Microsoft.AspNetCore.Mvc.ModelBinding;
using Server.Entities;
using Server.Models;


namespace Server.Utils;

public static class Helpers
{
    // public static PasswordManagerUserVM ToPasswordManagerUserVM(this PasswordmanagerUser dto)
    // {
    //     return new()
    //     {
    //         Id = dto.Id,
    //         Email = dto.Email,
    //         Firstname = dto.Firstname,
    //         Lastname = dto.Lastname,
    //         Datelastlogin = dto.Datelastlogin,
    //         Datelastlogout = dto.Datelastlogout,
    //         Datecreated = dto.Datecreated,
    //         Dateretired = dto.Dateretired,
    //     };
    // }

    // public static PasswordmanagerUser ToPasswordManagerUser(this PasswordManagerUserVM dto)
    // {
    //     return new()
    //     {
    //         Id = dto.Id,
    //         Email = dto.Email,
    //         Firstname = dto.Firstname,
    //         Lastname = dto.Lastname,
    //         Datelastlogin = dto.Datelastlogin,
    //         Datelastlogout = dto.Datelastlogout,
    //         Datecreated = dto.Datecreated,
    //         Dateretired = dto.Dateretired,
    //     };
    // }


    public static PasswordmanagerAccount ToPasswordManagerAccount(this PasswordAccountDTO dto)
    {
        return new()
        {
            // ignore assigning id because id here is an autogenerating primary key
            // Id = dto.Id!
            // ,
            Userid = dto.UserId!
            ,
            Title = dto.Title!
            ,
            Username = dto.Username!
            ,
            Password = dto.Password!
            ,
            CreatedAt = dto.CreatedAt
            ,
            LastUpdatedAt = dto.LastUpdatedAt
        };
    }

    public static PasswordAccountDTO ToPasswordManagerAccountDTO(this PasswordmanagerAccount account)
    {
        return new()
        {
            Id = account.Id
            ,
            UserId = account.Userid
            ,
            Title = account.Title
            ,
            Username = account.Username
            ,
            Password = account.Password
            ,
            CreatedAt = account.CreatedAt
            ,
            LastUpdatedAt = account.LastUpdatedAt
        };
    }

}
