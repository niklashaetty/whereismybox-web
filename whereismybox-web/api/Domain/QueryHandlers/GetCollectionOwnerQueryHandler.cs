using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetCollectionOwnerQueryHandler : IQueryHandler<GetCollectionOwnerQuery, User>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IUserRepository _userRepository;
    
    public GetCollectionOwnerQueryHandler(ICollectionRepository collectionRepository, IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        ArgumentNullException.ThrowIfNull(userRepository);
        _collectionRepository = collectionRepository;
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(GetCollectionOwnerQuery query)
    {
        var collection = await _collectionRepository.Get(query.CollectionId);
        var user = await _userRepository.Get(collection.Owner);
        return user;
    }
}