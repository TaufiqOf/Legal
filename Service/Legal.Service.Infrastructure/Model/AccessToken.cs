using Legal.Service.Infrastructure.Interface;

namespace Legal.Service.Infrastructure.Model;

public class AccessToken : IAccessToken
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Name { get; set; }

    public bool IsAdmin { get; set; }
}

public class ConnectionString
{
    public string? ConnectionStringValue { get; set; } = null!;

    public string? GisAppUrl { get; set; } = null!;

    public string? SecretKey { get; set; } = null!;

    public string? IscloudKey { get; set; } = null!;
}