namespace Domain.Services.BoxCreationService;

public class NonUniqueBoxException : Exception
{
    public NonUniqueBoxException(string message) : base(message)
    {
    }
}