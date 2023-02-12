using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.ItemAddingService;

public class ItemAddingService : IItemAddingService
{
    private readonly IBoxRepository _boxRepository;
    private readonly IUnattachedItemRepository _unattachedItemRepository;

    public ItemAddingService(IBoxRepository boxRepository, IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        _boxRepository = boxRepository;
        _unattachedItemRepository = unattachedItemRepository;
    }

    public async Task<Item> CreateItem(Guid userId, Guid boxId, string name, string description)
    {
        var item = Item.Create(name, description);
        var unattachedItemCollection = await GetOrCreateUnattachedItems(userId);
        unattachedItemCollection.Add(item);
        await _unattachedItemRepository.PersistUpdate(unattachedItemCollection);

        await AddUnattachedItemToBox(userId, boxId, item.ItemId);
        return item;
    }

    private async Task<UnattachedItemCollection> GetOrCreateUnattachedItems(Guid userId)
    {
        UnattachedItemCollection unattachedItemCollection;
        try
        {
            unattachedItemCollection = await _unattachedItemRepository.Get(userId);
        }
        catch (UnattachedItemsNotFoundException)
        {
            unattachedItemCollection = await _unattachedItemRepository.Create(UnattachedItemCollection.Create(userId));
        }

        return unattachedItemCollection;
    }


    public async Task<Item> AddUnattachedItemToBox(Guid userId, Guid boxId, Guid itemId)
    {
        var unattachedItems = await GetOrCreateUnattachedItems(userId);
        if (unattachedItems.TryGetItem(itemId, out var item))
        {
            var box = await _boxRepository.Get(userId, boxId);
            await AddItemToBox(userId, item, box);
            await RemoveUnattachedItemIfExists(userId, item.ItemId);
            return item;
        }
            
        throw new UnattachedItemNotFoundException($"Unattached item {itemId} not found for user {userId}");
    }

    private async Task AddItemToBox(Guid userId, Item item, Box box)
    {
        box.AddItem(item);
        await _boxRepository.PersistUpdate(userId, box);
    }

    private async Task RemoveUnattachedItemIfExists(Guid userId, Guid itemId)
    {
        var unattachedItems = await _unattachedItemRepository.Get(userId);
        if (unattachedItems.RemoveIfExists(itemId) > 0)
        { 
            await _unattachedItemRepository.PersistUpdate(unattachedItems);
        }
    }
}