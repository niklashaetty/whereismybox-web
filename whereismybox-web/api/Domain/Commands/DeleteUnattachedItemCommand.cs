using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteUnattachedItemCommand(CollectionId CollectionId, ItemId ItemId) : ICommand
{
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
}