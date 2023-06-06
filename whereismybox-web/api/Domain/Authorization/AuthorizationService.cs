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
        var user = await _userRepository.Get(externalUserId);

        if (user.PrimaryCollectionId.Equals(collectionId))
        {
            return;
        }

        throw new ForbiddenCollectionAccessException();
    }
}