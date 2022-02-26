using AppWeb.AuthorizationHandler;
using Business.Interfaces;
using Business.Services;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Database.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);

// Options Database Context
builder.Services.AddDbContext<MasterContext>(options =>
{
    var dbConnection = Environment.GetEnvironmentVariable("DBConnection") ?? builder.Configuration["DBConnection"];
    options.UseSqlServer(dbConnection);
});

// Security
builder.Services.AddAuthentication(options =>
    {
        //
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/AccessDenied");
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });
builder.Services.AddAuthorization();
builder.Services.AddTransient<IAuthorizationHandler, CustomAuthorizationHandler>();

// DataAccess Repositories
builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));

// Services
builder.Services.AddScoped<IConverterService, ConverterService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ITagService, TagService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();