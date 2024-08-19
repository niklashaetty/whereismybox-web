using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;

namespace NarrowIntegrationTests.Fakes;

public class FakeCollectionRepository : ICollectionRepository
{
    public Task<Collection> Create(Collection collection)
    {
        throw new NotImplementedException();
    }

    public Task Delete(CollectionId collectionId)
    {
        throw new NotImplementedException();
    }

    public Task<Collection> Get(CollectionId collectionId)
    {
        throw new NotImplementedException();
    }

    public Task<Collection> PersistUpdate(Collection collection)
    {
        throw new NotImplementedException();
    }

    public Task<List<Collection>> GetOwnedCollections(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Collection>> GetCollectionsWhereUserIsContributor(UserId userId)
    {
        throw new NotImplementedException();
    }
}