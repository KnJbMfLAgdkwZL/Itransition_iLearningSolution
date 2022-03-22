using System.Security.Claims;
using Business.Dto;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public class ClaimsService : IClaimsService
{
    private UserClaims GetClaims(dynamic context)
    {
        var claimUid = (Claim) context.User.FindFirst("Uid");
        var claimEmail = (Claim) context.User.FindFirst("Email");
        var claimNetwork = (Claim) context.User.FindFirst("Network");
        var claimRole = (Claim) context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);

        return new UserClaims()
        {
            Uid = claimUid == null ? string.Empty : claimUid.Value,
            Email = claimEmail == null ? string.Empty : claimEmail.Value,
            Network = claimNetwork == null ? string.Empty : claimNetwork.Value,
            Role = claimRole == null ? string.Empty : claimRole.Value
        };
    }

    public UserClaims GetClaims(AuthorizationHandlerContext context)
    {
        return GetClaims((object) context);
    }

    public UserClaims GetClaims(HttpContext context)
    {
        return GetClaims((object) context);
    }
}