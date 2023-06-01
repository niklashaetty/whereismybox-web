using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetBoxCollectionQueryHandler : IQueryHandler<GetBoxCollectionQuery, List<Box>>
{
    private readonly IBoxRepository _boxRepository;
    
    public GetBoxCollectionQueryHandler(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }
    
    public async Task<List<Box>> Handle(GetBoxCollectionQuery query)
    {
        return await _boxRepository.GetCollection(query.CollectionId);
    }
}