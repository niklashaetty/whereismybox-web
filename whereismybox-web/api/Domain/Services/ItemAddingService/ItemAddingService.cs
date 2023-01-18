using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.ItemAddingService;

public class ItemAddingService : IItemAddingService
{
    private readonly IBoxRepository _boxRepository;

    public ItemAddingService(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    public async Task<Item> AddItem(Guid userId, Guid boxId, string name, string description)
    {
        var box = await _boxRepository.Get(userId, boxId);

        var item = Item.Create(name, description);
        box.AddItem(item);
        await _boxRepository.PersistUpdate(userId, box);
        return item;
    }
}