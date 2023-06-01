using Domain.Models;
using Domain.Primitives;

namespace Domain.Repositories;

public interface ICollectionRepository
{
    public Task<Collection> CreateCollection(Collection collection);
    public Task<Collection> GetCollection(CollectionId collectionId);
    public Task<Collection> PersistUpdate(Collection updatedCollection);
}