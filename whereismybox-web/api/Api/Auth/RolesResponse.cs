namespace Api.Auth;

public class RolesResponse
{
    public List<string> Roles { get; set; }

    public RolesResponse(List<string> roles)
    {
        ArgumentNullException.ThrowIfNull(roles);
        Roles = roles;
    }
}