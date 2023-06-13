using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;

namespace Domain.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;

    public AuthorizationService(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task EnsureCollectionAccessAllowed(ExternalUserId externalUserId, CollectionId collectionId)
    {
        var user = await _userRepository.SearchByExternalUserId(externalUserId);


        if (user is null || IsOwnerOrContributor(user, collectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }
    }

    private static bool IsOwnerOrContributor(User user, CollectionId collectionId)
    {
        return user.PrimaryCollectionId.Equals(collectionId) ||
               user.ContributorCollections.Any(c => c.Equals(collectionId));
    }
}