using Domain.Primitives;

namespace Domain.Exceptions;

public class CollectionNotFoundException : Exception
{
    public CollectionNotFoundException(CollectionId collectionId) : base(
        $"No collection found with id {collectionId}")
    {
    }
}