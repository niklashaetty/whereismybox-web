using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record MoveItemCommand(UserId UserId, CollectionId CollectionId, ItemId ItemId,  BoxId SourceId, int TargetBoxNumber) : ICommand
{
    public readonly UserId UserId= UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
    public readonly BoxId SourceId = SourceId ?? throw new ArgumentNullException(nameof(SourceId));
    public readonly int TargetBoxNumber = TargetBoxNumber;
}