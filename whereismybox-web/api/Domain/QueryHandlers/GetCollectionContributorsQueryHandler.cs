using Domain.Authorization;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetCollectionContributorsQueryHandler : IQueryHandler<GetCollectionContributorsQuery, List<User>>
{
    private readonly IUserRepository _userRepository;
    
    public GetCollectionContributorsQueryHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }
    
    public async Task<List<User>> Handle(GetCollectionContributorsQuery query)
    {
        if (query.Permissions.IsOwner(query.Permissions.UserId, query.CollectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }
        
        return await _userRepository.SearchByCollectionId(query.CollectionId);
    }
}