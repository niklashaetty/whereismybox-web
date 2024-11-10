using Domain.Primitives;

namespace Domain.Exceptions;

public class BoxWithNumberNotFoundException : Exception
{
    public BoxWithNumberNotFoundException(CollectionId collectionId, int boxNumber) : base(
        $"No box with number {boxNumber} was found in collection {collectionId}")
    {
    }
}