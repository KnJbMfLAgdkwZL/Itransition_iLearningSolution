using Business.Interfaces;
using Business.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AppWeb.AuthorizationHandler;

public class CustomAuthorizationHandler : IAuthorizationHandler
{
    private readonly IUserSocialService _userSocialService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;

    public CustomAuthorizationHandler(IUserSocialService userSocialService, IUserClaimsService userClaimsService,
        IUserService userService)
    {
        _userSocialService = userSocialService;
        _userClaimsService = userClaimsService;
        _userService = userService;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var userClaims = _userClaimsService.GetClaims(context);
        var userSocial = await _userSocialService.Get(userClaims);
        if (userSocial != null)
        {
            var user = await _userService.GetUserBySocialId(userSocial.Id);
            if (user != null)
            {
                if (user.Role.Name == userClaims.Role && (user.Role.Name == "User" || user.Role.Name == "Admin"))
                {
                    context.Succeed(new AssertionRequirement(handlerContext => handlerContext.HasSucceeded));
                    return;
                }
            }
        }

        context.Fail();
    }
}