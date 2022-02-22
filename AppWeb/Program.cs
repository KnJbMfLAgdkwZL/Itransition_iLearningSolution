using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

Environment.GetEnvironmentVariable("Google_ClientId");

//services.AddMvc();
//builder.Services.AddControllersWithViews();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options => { options.LoginPath = "/account/google-login"; })
    .AddGoogle(googleOptions =>
    {
        var googleClientId = Environment.GetEnvironmentVariable("Google_ClientId") ??
                             builder.Configuration["Authentication:Google:ClientId"];
        var googleClientSecret = Environment.GetEnvironmentVariable("Google_ClientSecret") ??
                                 builder.Configuration["Authentication:Google:ClientSecret"];

        googleOptions.ClientId = "21145603976-h7dpvcbb4bieeqqt0j0v2k25p6iv807e.apps.googleusercontent.com";
        googleOptions.ClientSecret = "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";
        //googleOptions.CallbackPath = "/account/googleresponse";
    });

builder.Services.AddControllersWithViews();
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


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();