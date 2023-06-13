using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class User
{
    [JsonProperty] public UserId UserId { get; private set; }
    [JsonProperty] public ExternalUserId ExternalUserId { get; private set; }
    [JsonProperty] public string ExternalIdentityProvider { get; private set; }
    [JsonProperty] public string Username { get; private set; }
    [JsonProperty] public CollectionId PrimaryCollectionId { get; private set; }
    [JsonProperty] public List<CollectionId> ContributorCollections { get; private set; }

    [JsonConstructor]
    protected User()
    {
    }

    public User(UserId userId, ExternalUserId externalUserId, string externalIdentityProvider, string username,
        CollectionId primaryCollectionId, List<CollectionId> contributorCollections)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(externalUserId);
        ArgumentNullException.ThrowIfNull(externalIdentityProvider);
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(primaryCollectionId);
        UserId = userId;
        ExternalUserId = externalUserId;
        ExternalIdentityProvider = externalIdentityProvider;
        Username = username;
        PrimaryCollectionId = primaryCollectionId;
        ContributorCollections = contributorCollections ?? new List<CollectionId>();
    }

    public void AddAsContributor(CollectionId collectionId)
    {
        if (ContributorCollections.Any(c => c.Equals(collectionId)) is false)
        {
            ContributorCollections.Add(collectionId);
        }
    }
    
    public void RemoveAsContributor(CollectionId collectionId)
    {
        ContributorCollections.RemoveAll(c => c.Equals(collectionId));
    }
}