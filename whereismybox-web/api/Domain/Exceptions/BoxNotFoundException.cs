using Domain.Primitives;

namespace Domain.Exceptions;

public class BoxNotFoundException : Exception
{
    public BoxNotFoundException(CollectionId collectionId, BoxId boxId) : base(
        $"No box with id {boxId} was found in collection {collectionId}")
    {
    }
}