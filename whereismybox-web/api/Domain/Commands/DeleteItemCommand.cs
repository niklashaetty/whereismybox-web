using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteItemCommand(CollectionId CollectionId, BoxId BoxId, ItemId ItemId, bool IsHardDelete) : ICommand
{
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
    public bool IsHardDelete = IsHardDelete;
}