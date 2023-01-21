using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.ItemEditingService;

public class ItemEditingService : IItemEditingService
{
    private readonly IBoxRepository _boxRepository;

    public ItemEditingService(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    public async Task<Item> EditItem(Guid userId, Guid boxId, Item item)
    {
        var existingBox = await _boxRepository.Get(userId, boxId);
        existingBox.UpdateItem(item);
        await _boxRepository.PersistUpdate(userId, existingBox);
        return item;
    }
}