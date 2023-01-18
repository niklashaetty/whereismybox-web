using Domain.Repositories;

namespace Domain.Services.ItemDeletionService;

public class ItemDeletionService : IItemDeletionService
{
    private readonly IBoxRepository _boxRepository;
    
    public ItemDeletionService(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    public async Task DeleteItem(Guid userId, Guid boxId, Guid itemId)
    {
        var box = await _boxRepository.Get(userId, boxId);
        box.RemoveItem(itemId);
        await _boxRepository.PersistUpdate(userId, box);

    }
}