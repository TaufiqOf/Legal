using Microsoft.IdentityModel.Tokens;

namespace Legal.Service.Infrastructure.Model;

public sealed record class JwtBearerOptionsSettings
{
    public string Authority { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public bool RequireHttpsMetadata { get; init; } = false;
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;

    public TokenValidationParameters TokenValidationParameters { get; init; } = default!;
}