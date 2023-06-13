using Domain.Models;
using Domain.Primitives;

namespace Domain.Repositories;

public interface IUserRepository
{
    public Task<User> Create(User user);
    
    /// <summary>
    /// Gets a user
    /// </summary>
    /// <throws>UserNotFoundException</throws>
    public Task<User> Get(UserId userId);
    
    /// <summary>
    /// Returns null if not found
    /// </summary>
    public Task<User?> SearchByUsername(string username);
    
    /// <summary>
    /// Returns null if not found
    /// </summary>
    public Task<User?> SearchByExternalUserId(ExternalUserId externalUserId);
    
    /// <summary>
    /// Persist a user update
    /// </summary>
    /// <throws>UserNotFoundException</throws>
    public Task<User> PersistUpdate(User updatedUser);
    
    public Task<List<User>> SearchByCollectionId(CollectionId collectionId);
}