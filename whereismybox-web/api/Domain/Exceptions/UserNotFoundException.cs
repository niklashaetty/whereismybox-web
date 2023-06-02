using Domain.Primitives;

namespace Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(UserId userId) : base($"User with id {userId} not found")
    {
    }
    
    public UserNotFoundException(ExternalUserId externalUserId) : base($"User with externalId {externalUserId} not found")
    {
    }
    
    public UserNotFoundException(CollectionId collectionId) : base($"User with collectionId {collectionId} not found")
    {
    }
}