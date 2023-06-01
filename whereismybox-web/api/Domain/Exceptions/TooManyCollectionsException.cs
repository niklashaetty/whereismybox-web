using Domain.Primitives;

namespace Domain.Exceptions;

public class TooManyCollectionsException : Exception
{
    public TooManyCollectionsException(UserId userId, int amount) : base($"User {userId} has reached maximum amount of collections: {amount}")
    {
    }
}