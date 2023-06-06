using Domain.Primitives;

namespace Domain.Authorization;

public interface IAuthorizationService
{
    public Task EnsureCollectionAccessAllowed(ExternalUserId externalUserId, CollectionId collectionId);
}