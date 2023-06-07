using System.Security.Claims;

namespace Api.Auth;

public class RolesRequest
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; }
    public List<Claim> Claims { get; set; }
}
