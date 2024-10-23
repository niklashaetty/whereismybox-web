using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;

namespace Domain.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly ICollectionRepository _collectionRepository;

    public AuthorizationService(IUserRepository userRepository, ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _userRepository = userRepository;
        _collectionRepository = collectionRepository;
    }

    public async Task EnsureCollectionAccessAllowed(UserId userId, CollectionId collectionId)
    {
        var user = await _userRepository.Get(userId);
        var collection = await _collectionRepository.Get(collectionId);

        if (user is null || IsOwnerOrContributor(user, collection) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }
    }

    private static bool IsOwnerOrContributor(User user, Collection collection)
    {
        return collection.Owner.Equals(user.UserId) ||
               collection.Contributors.Any(u => u.Equals(user.UserId));
    }
}