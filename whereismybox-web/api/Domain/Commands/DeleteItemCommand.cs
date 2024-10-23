using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteItemCommand(UserId UserId, CollectionId CollectionId, BoxId BoxId, ItemId ItemId, bool IsHardDelete) : ICommand
{
    public readonly UserId UserId = UserId ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
    public readonly bool IsHardDelete = IsHardDelete;
}