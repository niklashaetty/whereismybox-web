namespace Infrastructure.BoxRepository;

public class BoxNotFoundException : Exception
{
    public BoxNotFoundException(Guid userId, Guid boxId) : base($"No box with id {boxId} was found on user {userId}")
    {
    }
}