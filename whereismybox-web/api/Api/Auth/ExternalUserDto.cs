namespace Api.Auth;

public class ExternalUserDto
{
    public string IdentityProvider { get; set; }
    public string UserId { get; set; }
    public string UserDetails { get; set; }
}