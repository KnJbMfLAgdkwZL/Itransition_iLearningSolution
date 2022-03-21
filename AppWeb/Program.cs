using AppWeb.AuthorizationHandler;
using Business.Dto.Options;
using Business.Interfaces;
using Business.Interfaces.Model;
using Business.Services;
using Business.Services.ModelServices;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Database.DbContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);

// Options
var uploadOptions = builder.Configuration.GetSection("UploadOptions");
uploadOptions["DropBoxAccessToken"] ??= Environment.GetEnvironmentVariable("DropBoxAccessToken");
uploadOptions["MegaEmail"] ??= Environment.GetEnvironmentVariable("MegaEmail");
uploadOptions["MegaPassword"] ??= Environment.GetEnvironmentVariable("MegaPassword");
uploadOptions["AzureStorageConnectionString"] ??= Environment.GetEnvironmentVariable("AzureStorageConnectionString");
builder.Services.Configure<UploadOptions>(uploadOptions);

// Options Database Context
builder.Services.AddDbContext<MasterContext>(options =>
{
    var dbConnection = Environment.GetEnvironmentVariable("DBConnection") ?? builder.Configuration["DBConnection"];
    options.UseSqlServer(dbConnection);
});

// Security
builder.Services.AddTransient<IAuthorizationHandler, CustomAuthorizationHandler>();
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

// DataAccess Repositories
builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));

// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IProductGroupService, ProductGroupService>();
builder.Services.AddScoped<IReviewLikeService, ReviewLikeService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewTagService, ReviewTagService>();
builder.Services.AddScoped<IReviewUserRatingService, ReviewUserRatingService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IStatusReviewService, StatusReviewService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserSocialService, UserSocialService>();
builder.Services.AddScoped<IUploadService, UploadService>();

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