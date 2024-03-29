using Newtonsoft.Json;

namespace Api.Auth;

public class RolesResponse
{
    [JsonProperty("roles")] public List<string> Roles { get; set; }

    public RolesResponse(List<string> roles)
    {
        ArgumentNullException.ThrowIfNull(roles);
        Roles = roles;
    }
}