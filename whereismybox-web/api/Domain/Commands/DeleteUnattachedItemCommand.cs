using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteUnattachedItemCommand(ExternalUserId ExternalUserId, CollectionId CollectionId, ItemId ItemId) : ICommand
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId  ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
}