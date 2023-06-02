using Domain.Models;
using Domain.Primitives;

namespace Domain.Repositories;

public interface IUnattachedItemRepository
{
    public Task<UnattachedItem> Create(UnattachedItem unattachedItem);
    
    /// <summary>
    /// Gets items that are not associated to a box
    /// </summary>
    public Task<List<UnattachedItem>> GetCollection(CollectionId collectionId);
    
    public Task Delete(CollectionId collectionId, ItemId itemId);
}