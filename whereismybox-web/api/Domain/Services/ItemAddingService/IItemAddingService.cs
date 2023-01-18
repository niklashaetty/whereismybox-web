using Domain.Models;

namespace Domain.Services.ItemAddingService;

public interface IItemAddingService
{
    public Task<Item> AddItem(Guid userId, Guid boxId, string name, string description);
}