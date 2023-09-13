using Domain.Authorization;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetCollectionOwnerQueryHandler : IQueryHandler<GetCollectionOwnerQuery, User>
{
    private readonly IUserRepository _userRepository;
    
    public GetCollectionOwnerQueryHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(GetCollectionOwnerQuery query)
    {
        return await _userRepository.GetCollectionOwner(query.CollectionId);
    }
}