using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteUnattachedItemCommand(UserId UserId, CollectionId CollectionId, ItemId ItemId) : ICommand
{
    public readonly UserId UserId = UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
}