using Domain.Primitives;

namespace Domain.Models;

public class Permissions
{
    public UserId UserId { get; }
    public List<Collection> OwnedCollections { get; }
    public List<Collection> ContributorCollections { get; }

    public Permissions(UserId userId, List<Collection> ownedCollections, List<Collection> contributorCollections)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(ownedCollections);
        ArgumentNullException.ThrowIfNull(contributorCollections);
        UserId = userId;
        OwnedCollections = ownedCollections;
        ContributorCollections = contributorCollections;
    }

    public bool IsContributor(UserId userId, CollectionId collectionId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(collectionId);
        if (UserId != userId)
        {
            return false;
        }
        return OwnedCollections.Any(c => c.CollectionId.Equals(collectionId)) ||
               ContributorCollections.Any(c => c.CollectionId.Equals(collectionId));
    }
    
    public bool IsOwner(UserId userId, CollectionId collectionId)
    {
        if (UserId != userId)
        {
            return false;
        }
        ArgumentNullException.ThrowIfNull(collectionId);
        return OwnedCollections.Any(c => c.CollectionId.Equals(collectionId));
    }
}