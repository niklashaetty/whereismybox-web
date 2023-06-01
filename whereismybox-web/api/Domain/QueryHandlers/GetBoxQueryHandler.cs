using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetBoxQueryHandler : IQueryHandler<GetBoxQuery, Box>
{
    private readonly IBoxRepository _boxRepository;
    
    public GetBoxQueryHandler(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }
    
    public async Task<Box> Handle(GetBoxQuery query)
    {
        return await _boxRepository.Get(query.CollectionId, query.BoxId);
    }
}