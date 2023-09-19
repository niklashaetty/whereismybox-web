using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetUserPermissionsQueryHandler : IQueryHandler<GetUserPermissionsQuery, Permissions>
{
    private readonly ICollectionRepository _collectionRepository;
    
    public GetUserPermissionsQueryHandler(ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _collectionRepository = collectionRepository;
    }
    
    public async Task<Permissions> Handle(GetUserPermissionsQuery query)
    {
        var contributorCollections = await _collectionRepository.GetCollectionsWhereUserIsContributor(query.UserId);
        var ownedCollections = await _collectionRepository.GetOwnedCollections(query.UserId);
        return new Permissions(query.UserId, ownedCollections, contributorCollections);
    }
}