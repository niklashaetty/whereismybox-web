using Domain.Models;

namespace Domain.Services.ItemEditingService;

public interface IItemEditingService
{
    public Task<Item> EditItem(Guid userId, Guid boxId, Item item);
}