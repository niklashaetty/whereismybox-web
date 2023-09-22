using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class User
{
    [JsonProperty] public UserId UserId { get; private set; }
    [JsonProperty] public ExternalUserId ExternalUserId { get; private set; }
    [JsonProperty] public string ExternalIdentityProvider { get; private set; }
    [JsonProperty] public string Username { get; private set; }

    [JsonProperty] public bool IsRegistered { get; private set; } = true;

    [JsonConstructor]
    protected User()
    {
    }

    public User(UserId userId, ExternalUserId externalUserId, string externalIdentityProvider, string username)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(externalUserId);
        ArgumentNullException.ThrowIfNull(externalIdentityProvider);
        ArgumentNullException.ThrowIfNull(username);
        UserId = userId;
        ExternalUserId = externalUserId;
        ExternalIdentityProvider = externalIdentityProvider;
        Username = username;
        IsRegistered = false;
    }

    public void RegisterUsername(string username)
    {
        Username = username;
        IsRegistered = true;
    }
}