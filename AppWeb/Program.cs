using Database.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
    .AddCookie(options => { });

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