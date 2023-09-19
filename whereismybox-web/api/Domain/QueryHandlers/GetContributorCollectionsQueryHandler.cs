using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetContributorCollectionsQueryHandler : IQueryHandler<GetContributorCollectionsQuery, List<Collection>>
{
    private readonly ICollectionRepository _collectionRepository;
    
    public GetContributorCollectionsQueryHandler(ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _collectionRepository = collectionRepository;
    }
    
    public async Task<List<Collection>> Handle(GetContributorCollectionsQuery query)
    {
        return await _collectionRepository.GetCollectionsWhereUserIsContributor(query.UserId);
    }
}