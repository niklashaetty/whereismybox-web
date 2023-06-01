using Domain.Primitives;

namespace Domain.Exceptions;

public class CollectionNotFoundException : Exception
{
    public CollectionNotFoundException(CollectionId collectionId) : base(
        $"No collection with id {collectionId} was found")
    {
    }
}