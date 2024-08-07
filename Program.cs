using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using LeoPasswordManager.Contexts;
using LeoPasswordManager.Interfaces;
using LeoPasswordManager.Repositories;
using LeoPasswordManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



// TODO: Adapt the app to use my ums

// old users will never get deleted. The userrole pairs they had will for the sake of simplicity
// dotnet ef dbcontext scaffold "Server=ep-shy-boat-a5z9pcbn.us-east-2.aws.neon.tech;Database=password_manager_db;User Id=neondb_owner;Password=NSFWkL9Zwb6f;Port=5432" Npgsql.EntityFrameworkCore.PostgreSQL -o Contexts

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
});

builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.HttpOnly = true;
    });

builder.Services.Configure<CookieAuthenticationOptions>(config =>
{
    config.AccessDeniedPath = new PathString("/AccessDenied");
    config.Events = new CookieAuthenticationEvents
    {
        OnRedirectToAccessDenied = context =>
        {
            // Custom logic when redirecting to access denied page
            context.Response.Redirect("/AccessDenied");
            return Task.CompletedTask;
        },
        // Add other event handlers as needed
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMIN", policy => policy.RequireRole("Admin"));
    // options.AddPolicy("USER", policy => policy.RequireRole("User"));
});

builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<UserManagementContext>(options =>
    options.UseNpgsql(
        builder.Environment.IsDevelopment()
                ?
                    builder.Configuration.GetConnectionString("UserManagementDB")
                :
                    Environment.GetEnvironmentVariable("UserManagementDB"))
    );

builder.Services.AddDbContext<PasswordManagerDbContext>(options =>
    options.UseNpgsql(
        builder.Environment.IsDevelopment()
                ?
                    builder.Configuration.GetConnectionString("DB_CONN")
                :
                    Environment.GetEnvironmentVariable("DB_CONN"))
    );

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
.AddEntityFrameworkStores<UserManagementContext>()
.AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = true;
});

builder.Services.AddSingleton<EncryptionContext>();
builder.Services.AddScoped<
    IPasswordManagerDbRepository<PasswordAccountModel>,
    PasswordManagerDbRepository>();

if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
    builder.WebHost.UseUrls($"http://*:{port}");
}

var app = builder.Build();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapBlazorHub();

app.Run();
