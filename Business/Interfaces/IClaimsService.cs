using Business.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IClaimsService
{
    UserClaims GetClaims(AuthorizationHandlerContext context);
    UserClaims GetClaims(HttpContext context);
}