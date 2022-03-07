using Business.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IUserClaimsService
{
    UserClaims GetClaims(AuthorizationHandlerContext context);
    UserClaims GetClaims(HttpContext context);
}