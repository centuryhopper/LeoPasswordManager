using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using LeoPasswordManager.Contexts;
using LeoPasswordManager.Interfaces;
using LeoPasswordManager.Models;
using LeoPasswordManager.Utilities;


namespace LeoPasswordManager.Repositories;

public class PasswordManagerAccountRepository : IPasswordManagerAccountRepository<PasswordAccountModel>
{
    private readonly EncryptionContext encryptionContext;
    private readonly ILogger<PasswordManagerAccountRepository> logger;
    private readonly PasswordAccountContext PasswordAccountContext;

    public PasswordManagerAccountRepository(EncryptionContext encryptionContext, ILogger<PasswordManagerAccountRepository> logger, PasswordAccountContext PasswordAccountContext)
    {
        this.encryptionContext = encryptionContext;
        this.logger = logger;
        this.PasswordAccountContext = PasswordAccountContext;
    }

    public async Task<PasswordAccountModel?> GetAccountModelAsync(string id, string userId)
    {
        var accountModel = await PasswordAccountContext.PasswordmanagerAccounts.FindAsync(id, userId);

        return accountModel?.ToPasswordManagerAccountModel();
    }

    public async Task<PasswordAccountModel?> CreateAsync(PasswordAccountModel model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password!));
        model.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd");
        model.LastUpdatedAt = DateTime.Now.ToString("yyyy-MM-dd");
        await PasswordAccountContext.PasswordmanagerAccounts.AddAsync(model.ToPasswordManagerAccount());
        await PasswordAccountContext.SaveChangesAsync();
        return model;
    }

    public async Task<PasswordAccountModel?> DeleteAsync(PasswordAccountModel model)
    {
        var queryModel = await PasswordAccountContext.PasswordmanagerAccounts.FindAsync(model.Id, model.UserId);
        PasswordAccountContext.PasswordmanagerAccounts.Remove(queryModel!);
        await PasswordAccountContext.SaveChangesAsync();
        return model;
    }

    public async Task<IEnumerable<PasswordAccountModel>> GetAllAccountsAsync(string userId)
    {
        var results = await PasswordAccountContext.PasswordmanagerAccounts.AsNoTracking().Where(a => a.Userid == userId).ToListAsync();
        // var results = await PasswordAccountContext.PasswordmanagerAccounts.AsNoTracking().ToListAsync();

        if (!results.Any())
        {
            return Enumerable.Empty<PasswordAccountModel>();
        }

        return results.Select(m =>
        {
            return new PasswordAccountModel
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

    public int AccountsCount(string UserId, string title)
    {
        var cnt = PasswordAccountContext.PasswordmanagerAccounts.Where(a => a.Userid == UserId && a.Title.ToLower().Contains(title)).Count();
        return cnt;
    }

    public async Task<PasswordAccountModel?> UpdateAsync(PasswordAccountModel model)
    {
        var dbModel = await PasswordAccountContext.PasswordmanagerAccounts.FindAsync(model.Id, model.UserId);
        dbModel!.LastUpdatedAt = DateTime.Now.ToString("yyyy-MM-dd");
        dbModel.Title = model.Title;
        dbModel.Username = model.Username;
        dbModel.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
        await PasswordAccountContext.SaveChangesAsync();

        return model;
    }

    public async Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid)
    {
        // set up csv helper and read file
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using var streamReader = new StreamReader(file.OpenReadStream());
        using var csvReader = new CsvReader(streamReader, config);
        IAsyncEnumerable<PasswordAccountModel> records;

        try
        {
            csvReader.Context.RegisterClassMap<PasswordsMapper>();
            records = csvReader.GetRecordsAsync<PasswordAccountModel>();

            await foreach (var record in records)
            {
                await CreateAsync(new PasswordAccountModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userid,
                    Title = record.Title,
                    Username = record.Username,
                    Password = record.Password,
                });
            }
        }
        catch (CsvHelperException ex)
        {
            return new UploadStatus
            {
                Message = "Failed to upload csv",
                UploadEnum = UploadEnum.FAIL
            };
        }

        return new UploadStatus
        {
            Message = "Upload csv success!",
            UploadEnum = UploadEnum.SUCCESS
        };
    }

}

