namespace Domain.Exceptions;

public class UnattachedItemsNotFoundException : Exception
{
    public UnattachedItemsNotFoundException(string message) : base(message)
    {
    }

    public UnattachedItemsNotFoundException(Guid userId, Exception innerException) : base(
        $"Unattached items for user {userId.ToString()} not found", innerException)
    {
    }
}