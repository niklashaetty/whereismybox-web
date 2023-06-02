using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class User
{
    [JsonProperty] public UserId UserId { get; private set; }
    [JsonProperty] public string UserName { get; private set; }
    [JsonProperty] public CollectionId PrimaryCollectionId { get; private set; }

    [JsonConstructor]
    protected User()
    {
    }
    
    public User(UserId userId, string userName, CollectionId primaryCollectionId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(primaryCollectionId);
        UserId = userId;
        UserName = userName;
        PrimaryCollectionId = primaryCollectionId;
    }
    
}