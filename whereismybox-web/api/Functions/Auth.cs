using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Api.Auth;
using Domain.Models;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;

namespace Functions;

public static class Auth
{
    public static ExternalUser ParseExternalUser(this HttpRequest req)
    {
        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header[0];
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            var principal = JsonSerializer.Deserialize<ExternalUserDto>(json,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            return new ExternalUser(new ExternalUserId(principal.UserId), principal.IdentityProvider,
                principal.UserDetails);
        }

        throw new UnparsableExternalUserException();
    }

    public static ExternalUser AsExternalUser(this RolesRequest request)
    {
        return new ExternalUser(new ExternalUserId(request.UserId), request.IdentityProvider,
            request.UserDetails);
    }

    public static RolesResponse AsRolesResponse(this User user)
    {
        var roles = new List<string> {"userId:" + user.UserId};
        return new RolesResponse(roles);
    }
}