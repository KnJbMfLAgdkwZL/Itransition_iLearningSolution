using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Business.Interfaces;

public interface IAccountService
{
    Task<bool> LoginOrRegister(string json, HttpContext httpContext, CancellationToken token);
    User? GetAuthorizedUser(HttpContext context, out IActionResult? error);
    User? GetAuthorizedUser(AuthorizationHandlerContext context, out IActionResult? error);
}