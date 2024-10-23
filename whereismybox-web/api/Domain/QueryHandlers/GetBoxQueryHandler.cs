using Domain.Authorization;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetBoxQueryHandler : IQueryHandler<GetBoxQuery, Box>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;
    
    public GetBoxQueryHandler(IAuthorizationService authorization, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _authorization = authorization;
        _boxRepository = boxRepository;
    }
    
    public async Task<Box> Handle(GetBoxQuery query)
    {
        await _authorization.EnsureCollectionAccessAllowed(query.UserId, query.CollectionId);
        return await _boxRepository.Get(query.CollectionId, query.BoxId);
    }
}