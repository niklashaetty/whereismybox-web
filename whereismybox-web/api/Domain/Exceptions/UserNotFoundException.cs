using Domain.Primitives;

namespace Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(UserId userId) : base($"User with id {userId} not found")
    {
    }
}