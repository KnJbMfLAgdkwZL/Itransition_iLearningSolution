using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AppWeb.AuthorizationHandler;

public class CustomAuthorizationHandler : IAuthorizationHandler
{
    private readonly IUserSocialService _userSocialService;

    public CustomAuthorizationHandler(IUserSocialService userSocialService)
    {
        _userSocialService = userSocialService;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var claimUid = context.User.FindFirst("Uid");
        var uid = claimUid == null ? string.Empty : claimUid.Value;

        var claimEmail = context.User.FindFirst("Email");
        var email = claimEmail == null ? string.Empty : claimEmail.Value;

        var claimNetwork = context.User.FindFirst("Network");
        var network = claimNetwork == null ? string.Empty : claimNetwork.Value;

        if (await _userSocialService.Get(uid, email, network) != null)
        {
            context.Succeed(new AssertionRequirement(handlerContext => handlerContext.HasSucceeded));
            return;
        }

        context.Fail();
    }
}