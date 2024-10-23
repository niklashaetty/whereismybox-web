using Domain.Primitives;

namespace Domain.Authorization;

public interface IAuthorizationService
{
    public Task EnsureCollectionAccessAllowed(UserId userId, CollectionId collectionId);
}