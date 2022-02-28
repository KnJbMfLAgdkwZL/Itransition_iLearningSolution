using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IAccountService
{
    Task<bool> LoginOrRegister(string json, HttpContext httpContext, CancellationToken token);
}