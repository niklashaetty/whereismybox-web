using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Domain.Models;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;

namespace Functions;

public static class Auth
{
    public class ExternalUserDto
    {
        public string IdentityProvider { get; set; }
        public string UserId { get; set; }
        public string UserDetails { get; set; }
    }

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
}