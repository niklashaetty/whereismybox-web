using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.ItemDeletionService;

public class ItemDeletionService : IItemDeletionService
{
    private readonly IBoxRepository _boxRepository;
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    
    public ItemDeletionService(IBoxRepository boxRepository, IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        _boxRepository = boxRepository;
        _unattachedItemRepository = unattachedItemRepository;
    }

    public async Task DeleteItem(Guid userId, Guid boxId, Guid itemId, bool isHardDelete)
    {
        var box = await _boxRepository.Get(userId, boxId);

        if (box.TryGetItem(itemId, out var item))
        {
            if (isHardDelete is false)
            {
                await AddItemToUnattached(userId, boxId, item);
            }
            
            box.RemoveItem(itemId);
            await _boxRepository.PersistUpdate(userId, box);
        }
    }

    private async Task AddItemToUnattached(Guid userId, Guid previousBoxId, Item item)
    {
        UnattachedItemCollection unattachedItems;
        try
        {
            unattachedItems = await _unattachedItemRepository.Get(userId);
        }
        catch (UnattachedItemsNotFoundException)
        {
            unattachedItems = await _unattachedItemRepository.Create(UnattachedItemCollection.Create(userId));
        }
        unattachedItems.Add(item, previousBoxId);
        await _unattachedItemRepository.PersistUpdate(unattachedItems);
    }
}