using Business.Dto;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public class UserClaimsService : IUserClaimsService
{
    public UserClaims GetClaims(AuthorizationHandlerContext context)
    {
        var claimUid = context.User.FindFirst("Uid");
        var claimEmail = context.User.FindFirst("Email");
        var claimNetwork = context.User.FindFirst("Network");
        return new UserClaims()
        {
            Uid = claimUid == null ? string.Empty : claimUid.Value,
            Email = claimEmail == null ? string.Empty : claimEmail.Value,
            Network = claimNetwork == null ? string.Empty : claimNetwork.Value
        };
    }

    public UserClaims GetClaims(HttpContext context)
    {
        var claimUid = context.User.FindFirst("Uid");
        var claimEmail = context.User.FindFirst("Email");
        var claimNetwork = context.User.FindFirst("Network");
        return new UserClaims()
        {
            Uid = claimUid == null ? string.Empty : claimUid.Value,
            Email = claimEmail == null ? string.Empty : claimEmail.Value,
            Network = claimNetwork == null ? string.Empty : claimNetwork.Value
        };
    }
}