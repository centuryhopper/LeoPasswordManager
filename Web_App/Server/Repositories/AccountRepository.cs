
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web.LayoutRenderers;
using Server.Contexts;
using Server.Entities;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Server.Repositories;

public class AccountRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, PasswordManagerDbContext passwordManagerDbContext) : IAccountRepository
{
    public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
    {
        if (loginDTO is null)
        {
            return new LoginResponse(false, null!, "Login container is empty");
        }

        var getUser = await userManager.FindByEmailAsync(loginDTO.Email);
        if (getUser is null)
        {
            // check email in passwordmanager users table and if it is there
            var passwordManagerUser = await passwordManagerDbContext.PasswordmanagerUsers.FirstOrDefaultAsync(u=>u.Email == loginDTO.Email);

            if (passwordManagerUser is null)
            {
                return new LoginResponse(false, null!, "User not found");
            }

            // then find the user in the ums by the ums id and overwrite the old email with the new one
            getUser = await userManager.FindByIdAsync(passwordManagerUser.UmsUserid);

            if (getUser is null)
            {
                return new LoginResponse(false, null!, "The user with this email was not found in the UMS.");
            }

            passwordManagerUser.Email = getUser.Email;
            await passwordManagerDbContext.SaveChangesAsync();
        }

        bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
        if (!checkUserPasswords)
        {
            return new LoginResponse(false, null!, "Invalid email/password");
        }

        // your custom new user insert goes here (below is just an example)

        // add user to stock user table if they arent in it already
        if (await passwordManagerDbContext.PasswordmanagerUsers.FirstOrDefaultAsync(u => u.UmsUserid == getUser.Id) is null)
        {
            try
            {
                await passwordManagerDbContext.PasswordmanagerUsers.AddAsync(new PasswordmanagerUser {
                    UmsUserid = getUser.Id!
                    ,Email = getUser.Email!
                    ,Datecreated = DateTime.Now
                    ,Datelastlogin = DateTime.Now
                    ,Dateretired = null
                });
                await passwordManagerDbContext.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                return new LoginResponse(false, null!, ex.Message);
            }
        }

        var stockUser = await passwordManagerDbContext.PasswordmanagerUsers.FirstOrDefaultAsync(u=>u.UmsUserid == getUser.Id);
        stockUser.Datelastlogin = DateTime.Now;
        await passwordManagerDbContext.SaveChangesAsync();

        var getUserRole = await userManager.GetRolesAsync(getUser);
        string token = GenerateToken(stockUser.Id, getUser.UserName, getUser.Email, getUserRole.First());

        return new LoginResponse(true, token!, "Login completed");
    }

    public async Task<GeneralResponse> Logout(int userId)
    {
        try
        {
            var stockUser = await passwordManagerDbContext.PasswordmanagerUsers.FindAsync(userId);
            stockUser.Datelastlogout = DateTime.Now;
            await passwordManagerDbContext.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            return new GeneralResponse(Flag: false, Message: ex.Message);
        }

        return new GeneralResponse(Flag: true, Message: "log out success!");
    }

    private string GenerateToken(int userId, string userName, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webHostEnvironment.IsDevelopment() ? configuration["Jwt:Key"] : Environment.GetEnvironmentVariable("Jwt_Key")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: webHostEnvironment.IsDevelopment() ? configuration["Jwt:Issuer"] : Environment.GetEnvironmentVariable("Jwt_Issuer"),
            audience: webHostEnvironment.IsDevelopment() ? configuration["Jwt:Audience"] : Environment.GetEnvironmentVariable("Jwt_Audience"),
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

