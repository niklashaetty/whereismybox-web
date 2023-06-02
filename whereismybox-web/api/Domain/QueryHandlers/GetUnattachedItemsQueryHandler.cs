using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetUnattachedItemsQueryHandler : IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>>
{
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly IBoxRepository _boxRepository;
    
    public GetUnattachedItemsQueryHandler(IUnattachedItemRepository unattachedItemRepository, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _unattachedItemRepository = unattachedItemRepository;
        _boxRepository = boxRepository;
    }

    public async Task<List<UnattachedItem>> Handle(GetUnattachedItemsQuery query)
    {
        var unattachedItems = await _unattachedItemRepository.GetCollection(query.CollectionId);
        await AttachPreviousBoxNumber(query, unattachedItems);
        return unattachedItems;
    }

    private async Task AttachPreviousBoxNumber(GetUnattachedItemsQuery query, List<UnattachedItem> unattachedItems)
    {
        foreach (var unattachedItem in unattachedItems)
        {
            if (unattachedItem.PreviousBoxId is not null)
            {
                try
                {
                    var box = await _boxRepository.Get(query.CollectionId, unattachedItem.PreviousBoxId);
                    unattachedItem.AddPreviousBoxNumber(box.Number);
                }
                catch (BoxNotFoundException e)
                {
                    // Then box doesn't exist anymore.
                    unattachedItem.RemovePreviousBox();
                }
            }
        }
    }
}