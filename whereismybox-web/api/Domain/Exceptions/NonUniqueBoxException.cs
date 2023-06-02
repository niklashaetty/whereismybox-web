namespace Domain.Exceptions;

public class NonUniqueBoxException : Exception
{
    public NonUniqueBoxException(string message) : base(message)
    {
    }
}