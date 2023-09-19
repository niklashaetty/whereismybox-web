using Domain.Authorization;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetCollectionContributorsQueryHandler : IQueryHandler<GetCollectionContributorsQuery, List<User>>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IUserRepository _userRepository;
    
    public GetCollectionContributorsQueryHandler(ICollectionRepository collectionRepository, IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        ArgumentNullException.ThrowIfNull(userRepository);
        _collectionRepository = collectionRepository;
        _userRepository = userRepository;
    }
    
    public async Task<List<User>> Handle(GetCollectionContributorsQuery query)
    {
        if (query.Permissions.IsOwner(query.Permissions.UserId, query.CollectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }

        var users = new List<User>();
        var collection = await _collectionRepository.Get(query.CollectionId);
        foreach (var userId in collection.Contributors)
        {
            users.Add(await _userRepository.Get(userId));
        }

        return users;
    }
}