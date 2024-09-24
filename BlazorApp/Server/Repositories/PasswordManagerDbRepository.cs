using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Server.Contexts;
using Server.Repositories;
using Shared.Models;
using Server.Entities;
using Shared.Models;
using Server.Utils;
using static Shared.Models.ServiceResponses;
using System.Security.Claims;


namespace Server.Repositories;

public class PasswordManagerDbRepository(EncryptionContext encryptionContext, ILogger<PasswordManagerDbRepository> logger, PasswordManagerDbContext passwordManagerDbContext, IHttpContextAccessor httpContextAccessor) : IPasswordManagerDbRepository
{
    public async Task<PasswordAccountDTO?> GetPasswordRecordAsync(int passwordRecordId)
    {
        var accountModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(passwordRecordId);
        return accountModel?.ToPasswordManagerAccountDTO();
    }


    public int AccountsCount(int UserId, string title)
    {
        var cnt = passwordManagerDbContext.PasswordmanagerAccounts.Where(a => a.Userid == UserId && a.Title.ToLower().Contains(title)).Count();
        return cnt;
    }

    public async Task<GeneralResponse> UploadCsvAsync(IFormFile file, int userId)
    {
        // set up csv helper and read file
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,

            // set to null to allow files with only title, usernam, and password headers to be uploaded
            HeaderValidated = null,
            MissingFieldFound = null,
        };


        using var streamReader = new StreamReader(file.OpenReadStream());
        using var csvReader = new CsvReader(streamReader, config);
        IAsyncEnumerable<PasswordAccountDTO> records;

        try
        {
            csvReader.Context.RegisterClassMap<PasswordsMapper>();
            records = csvReader.GetRecordsAsync<PasswordAccountDTO>();

            await foreach (var record in records)
            {
                await CreateAsync(new PasswordAccountDTO
                {
                    UserId = userId,
                    Title = record.Title,
                    Username = record.Username,
                    Password = record.Password,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                });
            }
        }
        catch (CsvHelperException ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }

        return new GeneralResponse(Flag: true, Message: "File uploaded!");

    }

    // public async Task<PasswordManagerUserVM?> UpdatePasswordManagerUser(PasswordManagerUserVM vm)
    // {
    //     try
    //     {
    //         var passwordManagerUser = await passwordManagerDbContext.PasswordmanagerUsers.FindAsync(vm.Id);
    //         passwordManagerUser!.Firstname = vm.Firstname;
    //         passwordManagerUser.Lastname = vm.Lastname;
    //         await passwordManagerDbContext.SaveChangesAsync();

    //         return passwordManagerUser?.ToPasswordManagerUserVM();
    //     }
    //     catch (System.Exception ex)
    //     {
    //         return null;
    //     }
    // }

    // public async Task<PasswordManagerUserVM?> GetPasswordManagerUser(string umsUserId)
    // {
    //     var passwordManagerUser = await passwordManagerDbContext.PasswordmanagerUsers.FirstOrDefaultAsync(u => u.UmsUserid == umsUserId);

    //     return passwordManagerUser?.ToPasswordManagerUserVM();
    // }

    // public async Task<PasswordManagerUserVM?> CreatePasswordManagerUser(ApplicationUser umsUser)
    // {
    //     PasswordmanagerUser passwordManagerUser = new()
    //     {
    //         Email = umsUser.Email,
    //         Firstname = "test_firstname",
    //         Lastname = "test_firstname",
    //         Datelastlogin = DateTime.Now,
    //         Datelastlogout = null,
    //         Datecreated = DateTime.Now,
    //         Dateretired = null,
    //         UmsUserid = umsUser.Id,
    //     };

    //     await passwordManagerDbContext.PasswordmanagerUsers.AddAsync(passwordManagerUser);
    //     await passwordManagerDbContext.SaveChangesAsync();

    //     return passwordManagerUser.ToPasswordManagerUserVM();
    // }

    public async Task<IEnumerable<PasswordAccountDTO>> GetAllPasswordRecordsAsync(int userId)
    {
        var results = await passwordManagerDbContext.PasswordmanagerAccounts.AsNoTracking().Where(a => a.Userid == userId).ToListAsync();

        if (!results.Any())
        {
            return Enumerable.Empty<PasswordAccountDTO>();
        }

        return results.Select(m =>
        {
            return new PasswordAccountDTO
            {
                Id = m.Id,
                Title = m.Title,
                Username = m.Username,
                Password = encryptionContext.Decrypt(Convert.FromBase64String(m.Password)).Replace(",", "$"),
                UserId = m.Userid,
                CreatedAt = m.CreatedAt,
                LastUpdatedAt = m.LastUpdatedAt
            };
        });
    }

    public async Task<GeneralResponse> CreateAsync(PasswordAccountDTO model)
    {
        model.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password!));
        model.CreatedAt = DateTime.Now;
        model.LastUpdatedAt = DateTime.Now;
        try
        {
            await passwordManagerDbContext.PasswordmanagerAccounts.AddAsync(model.ToPasswordManagerAccount());
            await passwordManagerDbContext.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }
        return new GeneralResponse(Flag: true, Message: "Password record created!");

    }

    public async Task<GeneralResponse> UpdateAsync(PasswordAccountDTO model)
    {
        var dbModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(model.Id);
        try
        {
            dbModel!.LastUpdatedAt = DateTime.Now;
            dbModel.Title = model.Title;
            dbModel.Username = model.Username;
            dbModel.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
            await passwordManagerDbContext.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }
        return new GeneralResponse(Flag: true, Message: "Password Record Updated!");
    }

    public async Task<GeneralResponse?> DeleteAsync(int passwordRecordId)
    {
        var queryModel = await passwordManagerDbContext.PasswordmanagerAccounts.FindAsync(passwordRecordId);

        if (queryModel is null)
        {
            return null;
        }

        passwordManagerDbContext.PasswordmanagerAccounts.Remove(queryModel!);

        try
        {
            await passwordManagerDbContext.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }

        return new GeneralResponse(Flag: true, Message: "Password Record Deleted!");
    }

}
