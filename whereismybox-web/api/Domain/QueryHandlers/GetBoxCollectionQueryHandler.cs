using Domain.Authorization;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetBoxCollectionQueryHandler : IQueryHandler<GetBoxCollectionQuery, List<Box>>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;
    
    public GetBoxCollectionQueryHandler(IAuthorizationService authorization, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(boxRepository);
    
        _boxRepository = boxRepository;
        _authorization = authorization;
    }
    
    public async Task<List<Box>> Handle(GetBoxCollectionQuery query)
    {
        await _authorization.EnsureCollectionAccessAllowed(query.ExternalUserId, query.CollectionId);
        return await _boxRepository.GetCollection(query.CollectionId);
    }
}