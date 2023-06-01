using Domain.Primitives;

namespace Domain.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(BoxId boxId, ItemId itemId) : base(
        $"Item {itemId} not found in box {boxId}")
    {
    }
}