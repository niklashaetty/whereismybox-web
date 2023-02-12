namespace Domain.Services.ItemAddingService;

public class UnattachedItemNotFoundException : Exception
{
    public UnattachedItemNotFoundException(string message) : base(message)
    {
    }
}