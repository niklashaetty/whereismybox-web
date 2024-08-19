using System.Text;
using Api.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NarrowIntegrationTests;

public static class RequestHelper
{
    public static HttpRequest CreateHttpRequest(object? body = null)
    {
        var context = new DefaultHttpContext();
        HttpRequest request = context.Request;
        var jsonBody = JsonConvert.SerializeObject(body);
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));

        return request;
    }
    
    public static void AddExternallyAuthenticatedHeader(this HttpRequest httpRequest, String externalUserId)
    {
        var externalUserDto = new ExternalUserDto
        {
            UserDetails = "Username",
            UserId = externalUserId,
            IdentityProvider = "identityProvider",
            UserRoles = new List<string> {"anonymous", "authenticated"}
        };

        AddClientPrincipalHeader(httpRequest, externalUserDto);
    }
    
    public static void AddFullyAuthenticatedUser(this HttpRequest httpRequest, string externalUserId, string internalUserId)
    {
        var externalUserDto = new ExternalUserDto
        {
            UserDetails = "Username",
            UserId = externalUserId,
            IdentityProvider = "identityProvider",
            UserRoles = new List<string> {"anonymous", "authenticated", "userId." + internalUserId}
        };

        AddClientPrincipalHeader(httpRequest, externalUserDto);
    }

    private static void AddClientPrincipalHeader(HttpRequest httpRequest, ExternalUserDto externalUserDto)
    {
        var jsonString = JsonConvert.SerializeObject(externalUserDto);
        var byteArray = Encoding.UTF8.GetBytes(jsonString);
        var encoded = Convert.ToBase64String(byteArray);
        httpRequest.Headers.Add("x-ms-client-principal", encoded);
    }
}