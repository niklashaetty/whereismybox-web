using Domain.Primitives;

namespace Domain.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(CollectionId collectionId, ItemId itemId) : base(
        $"No box with id {itemId} was found in collection {collectionId}")
    {
    }
}