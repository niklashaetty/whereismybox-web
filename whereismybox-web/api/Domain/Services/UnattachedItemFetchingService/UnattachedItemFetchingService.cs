using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.UnattachedItemFetchingService;

public class UnattachedItemFetchingService : IUnattachedItemFetchingService
{
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly IBoxRepository _boxRepository;

    public UnattachedItemFetchingService(IUnattachedItemRepository unattachedItemRepository,
        IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _unattachedItemRepository = unattachedItemRepository;
        _boxRepository = boxRepository;
    }

    public async Task<UnattachedItemCollection> Get(Guid userId)
    {
        var unattachedItemCollection = await _unattachedItemRepository.Get(userId);
        await AttachPreviousBoxNumber(userId, unattachedItemCollection);
        return unattachedItemCollection;
    }

    private async Task AttachPreviousBoxNumber(Guid userId, UnattachedItemCollection unattachedItemCollection)
    {
        foreach (var unattachedItem in unattachedItemCollection.UnattachedItems)
        {
            if (unattachedItem.PreviousBoxId.HasValue)
            {
                var box = await _boxRepository.Get(userId, unattachedItem.PreviousBoxId.Value);
                unattachedItem.AddPreviousBoxNumber(box.Number);
            }
        }
    }
}