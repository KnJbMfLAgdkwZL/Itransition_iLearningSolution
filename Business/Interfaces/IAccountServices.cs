using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IAccountServices
{
    Task<bool> LoginOrRegister(string json, HttpContext httpContext, CancellationToken token);
    Task<bool> GetUserSocial(string uid, string email, string network);
}