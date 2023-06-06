using Domain.Primitives;

namespace Domain.Models;

public class ExternalUser
{
    public ExternalUserId ExternalUserId { get; private set; }
    public string ExternalIdentityProvider { get; private set; }
    public string Username { get; private set; }

    public ExternalUser(ExternalUserId externalUserId, string externalIdentityProvider, string username)
    {
        ArgumentNullException.ThrowIfNull(externalUserId);
        ArgumentNullException.ThrowIfNull(externalIdentityProvider);
        ArgumentNullException.ThrowIfNull(username);
        ExternalUserId = externalUserId;
        ExternalIdentityProvider = externalIdentityProvider;
        Username = username;
    }
}