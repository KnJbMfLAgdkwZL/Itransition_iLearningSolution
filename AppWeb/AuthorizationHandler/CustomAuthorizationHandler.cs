using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AppWeb.AuthorizationHandler;

public class CustomAuthorizationHandler : IAuthorizationHandler
{
    private readonly IUserSocialService _userSocialService;
    private readonly IUserClaimsService _userClaimsService;

    public CustomAuthorizationHandler(IUserSocialService userSocialService, IUserClaimsService userClaimsService)
    {
        _userSocialService = userSocialService;
        _userClaimsService = userClaimsService;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var userClaims = _userClaimsService.GetClaims(context);
        if (await _userSocialService.Get(userClaims) != null)
        {
            context.Succeed(new AssertionRequirement(handlerContext => handlerContext.HasSucceeded));
            return;
        }

        context.Fail();
    }
}