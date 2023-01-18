using Domain.Models;

namespace Domain.Repositories;

public interface IUserRepository
{
    public Task<User> Create(User user);
    
    /// <summary>
    /// Gets a user
    /// </summary>
    /// <throws>UserNotFoundException</throws>
    public Task<User> Get(Guid userId);

    /// <summary>
    /// Persist a user update
    /// </summary>
    /// <throws>UserNotFoundException</throws>
    public Task<User> PersistUpdate(User updatedUser);
}