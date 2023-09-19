using Domain.Models;
using Domain.Primitives;

namespace Domain.Repositories;

public interface ICollectionRepository
{
    
    /// <summary>
    /// Create a new collection
    /// </summary>
    /// <throws>CollectionAlreadyExistsException</throws>
    public Task<Collection> Create(Collection collection);
    
    public Task Delete(CollectionId collectionId);

    /// <summary>
    /// Gets a box
    /// </summary>
    /// <throws>CollectionNotFoundException</throws>
    public Task<Collection> Get(CollectionId collectionId);

    /// <summary>
    /// Persist a box update
    /// </summary>
    /// <throws>CollectionNotFoundException</throws>
    public Task<Collection> PersistUpdate(Collection collection);
    
    /// <summary>
    /// List all collections where the user is owner
    /// </summary>
    public Task<List<Collection>> GetOwnedCollections(UserId userId);
    
    /// <summary>
    /// List all collections where the user is contributor
    /// </summary>
    public Task<List<Collection>> GetCollectionsWhereUserIsContributor(UserId userId);
}