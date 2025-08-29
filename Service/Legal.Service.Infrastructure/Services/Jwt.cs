using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace Legal.Service.Infrastructure.Services;

public static class Jwt
{
    public static IAccessToken DecodeJwt(this HttpContext? context)
    {
        if (context?.Request?.Headers == null)
        {
            return null;
        }

        context.Request.Headers.TryGetValue(HeaderNames.Authorization, out StringValues token);

        if (token.Count == 0
            || token[0] is null
            || token[0].StartsWith("Bearer null")
            || token[0].StartsWith("Bearer ") == false)
        {
            return null;
        }

        return Decode(token);

        // return new AccessToken()
        // {
        //     Name = token,
        //     UserName = token,
        //     UserId = token,
        // };
    }

    private static IAccessToken Decode(string token)
    {
        if (token is null)
        {
            return default;
        }

        string? payload = token.Split('.')[1];
        byte[] bytes = Base64UrlDecode(payload);
        var payloadJson = Encoding.UTF8.GetString(bytes);

        var result = JsonConvert.DeserializeObject<AccessToken>(payloadJson);
        return result;
    }

    // From JWT spec
    private static byte[] Base64UrlDecode(string input)
    {
        if (input is null)
        {
            return default;
        }

        string output = input
            .Replace('-', '+') // 62nd char of encoding
            .Replace('_', '/'); // 63rd char of encoding

        // Pad with trailing '='s
        output += (output.Length % 4) switch
        {
            0 => string.Empty, // No pad chars in this case
            2 => "==", // Two pad chars
            3 => "=", // One pad char
            _ => throw new Exception("Token isn't valid.")
        };

        return Convert.FromBase64String(output); // Standard base64 decoder
    }
}