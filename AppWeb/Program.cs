using Database.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
    })
    .AddGoogle(googleOptions =>
    {
        var clientId = Environment.GetEnvironmentVariable("Authentication:Google:ClientId") ??
                       builder.Configuration["Authentication:Google:ClientId"];
        var clientSecret = Environment.GetEnvironmentVariable("Authentication:Google:ClientSecret") ??
                           builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.ClientId = clientId;
        googleOptions.ClientSecret = clientSecret;
        
        googleOptions.CallbackPath = "/signin-google";
    })
    .AddFacebook(facebookOptions =>
    {
        var appId = Environment.GetEnvironmentVariable("Authentication:Facebook:AppId") ??
                    builder.Configuration["Authentication:Facebook:AppId"];
        var appSecret = Environment.GetEnvironmentVariable("Authentication:Facebook:AppSecret") ??
                        builder.Configuration["Authentication:Facebook:AppSecret"];
        facebookOptions.AppId = appId;
        facebookOptions.AppSecret = appSecret;
        
        facebookOptions.CallbackPath = "/signin-facebook";
    });

// Options Database Context
builder.Services.AddDbContext<MasterContext>(options =>
{
    var dbConnection = Environment.GetEnvironmentVariable("DBConnection") ?? builder.Configuration["DBConnection"];
    options.UseSqlServer(dbConnection);
});

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ProtectedSessionStorage>();


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();