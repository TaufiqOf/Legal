using Legal.Service.Infrastructure.Interface;

namespace Legal.Service.Infrastructure.Services;

public sealed class AuthorizeAttribute : Attribute
{
    public AuthorizeAttribute(IAccessToken accessToken)
    {
    }
}