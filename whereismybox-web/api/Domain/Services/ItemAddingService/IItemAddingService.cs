using Domain.Models;

namespace Domain.Services.ItemAddingService;

public interface IItemAddingService
{
    /// <summary>
    /// Creates a new item and adds as unattached
    /// </summary>
    public Task<Item> CreateItem(Guid userId, Guid boxId, string name, string description);
    
    /// <summary>
    /// Adds unattached item to box
    /// </summary>
    /// <throws>UnattachedItemNotFoundException if item does is not unattached</throws>
    public Task<Item> AddUnattachedItemToBox(Guid userId, Guid boxId, Guid itemId);
}