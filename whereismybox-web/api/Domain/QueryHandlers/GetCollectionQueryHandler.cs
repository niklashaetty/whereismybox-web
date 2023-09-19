using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetCollectionQueryHandler : IQueryHandler<GetCollectionQuery, Collection>
{
    private readonly ICollectionRepository _collectionRepository;
    
    public GetCollectionQueryHandler(ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _collectionRepository = collectionRepository;
    }
    
    public async Task<Collection> Handle(GetCollectionQuery query)
    {
        return await _collectionRepository.Get(query.CollectionId);
    }
}