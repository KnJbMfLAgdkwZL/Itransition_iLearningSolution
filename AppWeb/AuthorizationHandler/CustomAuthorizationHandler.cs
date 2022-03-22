using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AppWeb.AuthorizationHandler;

public class CustomAuthorizationHandler : IAuthorizationHandler
{
    private readonly IClaimsService _claimsService;
    private readonly IAccountService _accountService;

    public CustomAuthorizationHandler(
        IClaimsService claimsService,
        IAccountService accountService
    )
    {
        _claimsService = claimsService;
        _accountService = accountService;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var roles = new List<string>()
        {
            "User",
            "Admin"
        };
        
        var userClaims = _claimsService.GetClaims(context);
        
        var user = _accountService.GetAuthorizedUser(context, out var error, CancellationToken.None);
        if (user != null)
        {
            if (user.Role.Name == userClaims.Role && roles.Contains(user.Role.Name))
            {
                context.Succeed(new AssertionRequirement(handlerContext => handlerContext.HasSucceeded));
                return Task.CompletedTask;
            }
        }

        context.Fail();
        
        return Task.CompletedTask;
    }
}