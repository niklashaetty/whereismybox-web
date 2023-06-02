using Domain.Models;
using Domain.Primitives;

namespace Domain.Repositories;

public interface IBoxRepository
{
    public Task<Box> Add(Box box);
    
    public Task Delete(CollectionId collectionId, BoxId boxId);

    /// <summary>
    /// Gets a box
    /// </summary>
    /// <throws>BoxNotFoundException</throws>
    public Task<Box> Get(CollectionId collectionId, BoxId boxId);

    /// <summary>
    /// Persist a box update
    /// </summary>
    /// <throws>BoxNotFoundException</throws>
    public Task<Box> PersistUpdate(Box updatedBox);
    
    /// <summary>
    /// List all boxes in a collection
    /// </summary>
    public Task<List<Box>> GetCollection(CollectionId collectionId);
}