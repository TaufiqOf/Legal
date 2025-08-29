using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Legal.Service.Infrastructure.Helper;

public static class JwtTokenGenerator
{
    public static string GenerateJwtToken(string username, bool isAdmin)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", username),
            new Claim("UserName", username),
            new Claim("Email", $"{username}@example.com"),
            new Claim("Name", username),
            new Claim("IsAdmin", isAdmin.ToString())
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String("Rk5wYXRJUmNId2g0dW5aU01ZV1FUdXh0M1JaeHpHVWc="));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: username,
            audience: "your_audience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}