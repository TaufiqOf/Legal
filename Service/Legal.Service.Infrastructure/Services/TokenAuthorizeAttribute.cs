using Legal.Service.Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Legal.Service.Infrastructure.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TokenAuthorizeAttribute : TypeFilterAttribute
{
    public TokenAuthorizeAttribute() : base(typeof(TokenAuthorizeFilter))
    {
        // You can pass arguments to the filter here if needed
    }
}

public class TokenAuthorizeFilter : IAuthorizationFilter
{
    private readonly IAccessToken _accessToken;

    public TokenAuthorizeFilter(IAccessToken accessToken)
    {
        _accessToken = accessToken;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (_accessToken is null) // Your custom logic
        {
            context.Result = new UnauthorizedResult();
        }
    }
}