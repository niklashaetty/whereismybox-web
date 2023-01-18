using Domain.Models;

namespace Domain.Repositories;

public interface IBoxRepository
{
    public Task<Box> Add(Guid userId, Box box);
    
    /// <summary>
    /// Gets a box
    /// </summary>
    /// <throws>BoxNotFoundException</throws>
    public Task<Box> Get(Guid userId, Guid boxId);

    /// <summary>
    /// Persist a box update
    /// </summary>
    /// <throws>BoxNotFoundException</throws>
    public Task<Box> PersistUpdate(Guid userId, Box updatedBox);
    
    /// <summary>
    /// List all boxes related to a user
    /// </summary>
    public Task<List<Box>> ListBoxesByUser(Guid userId);
}