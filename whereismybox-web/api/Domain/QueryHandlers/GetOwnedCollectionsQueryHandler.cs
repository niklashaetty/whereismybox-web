using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetOwnedCollectionsQueryHandler : IQueryHandler<GetOwnedCollectionQuery, List<Collection>>
{
    private readonly ICollectionRepository _collectionRepository;
    
    public GetOwnedCollectionsQueryHandler(ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _collectionRepository = collectionRepository;
    }
    
    public async Task<List<Collection>> Handle(GetOwnedCollectionQuery query)
    {
        return await _collectionRepository.GetOwnedCollections(query.UserId);
    }
}