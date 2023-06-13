using Domain.Primitives;

namespace Domain.Models;

public class Permissions
{
    public UserId UserId { get; }
    public CollectionId PrimaryCollectionId { get; }
    public List<CollectionId> ContributorCollections { get; }

    public Permissions(UserId userId, CollectionId primaryCollectionId, List<CollectionId> contributorCollections)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(primaryCollectionId);
        ArgumentNullException.ThrowIfNull(contributorCollections);
        UserId = userId;
        PrimaryCollectionId = primaryCollectionId;
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
        return collectionId.Equals(PrimaryCollectionId) ||
               ContributorCollections.Any(c => c.Equals(collectionId));
    }
    
    public bool IsOwner(UserId userId, CollectionId collectionId)
    {
        if (UserId != userId)
        {
            return false;
        }
        ArgumentNullException.ThrowIfNull(collectionId);
        return collectionId.Equals(PrimaryCollectionId);
    }
}