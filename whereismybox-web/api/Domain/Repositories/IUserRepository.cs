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
    /// Persist a user update
    /// </summary>
    /// <throws>UserNotFoundException</throws>
    public Task<User> PersistUpdate(User updatedUser);
}