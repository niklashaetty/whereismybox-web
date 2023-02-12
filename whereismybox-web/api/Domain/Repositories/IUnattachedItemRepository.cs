using Domain.Models;

namespace Domain.Repositories;

public interface IUnattachedItemRepository
{
    public Task<UnattachedItemCollection> Create(UnattachedItemCollection unattachedItemCollection);
    
    /// <summary>
    /// Gets items that are not associated to a box
    /// </summary>
    public Task<UnattachedItemCollection> Get(Guid userId);
    
    public Task<UnattachedItemCollection> PersistUpdate(UnattachedItemCollection unattachedItemCollection);
}