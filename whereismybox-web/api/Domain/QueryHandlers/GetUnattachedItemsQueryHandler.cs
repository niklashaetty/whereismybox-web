using Domain.Authorization;
using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetUnattachedItemsQueryHandler : IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>>
{
    private readonly IAuthorizationService _authorization;
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly IBoxRepository _boxRepository;
    
    public GetUnattachedItemsQueryHandler(IAuthorizationService authorization, IUnattachedItemRepository unattachedItemRepository, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _authorization = authorization;
        _unattachedItemRepository = unattachedItemRepository;
        _boxRepository = boxRepository;
    }

    public async Task<List<UnattachedItem>> Handle(GetUnattachedItemsQuery query)
    {
        await _authorization.EnsureCollectionAccessAllowed(query.UserId, query.CollectionId);
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