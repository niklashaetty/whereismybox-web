using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record MoveItemCommand(ExternalUserId ExternalUserId, CollectionId CollectionId, ItemId ItemId,  BoxId SourceId, BoxId TargetId) : ICommand
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId  ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
    public readonly BoxId SourceId = SourceId ?? throw new ArgumentNullException(nameof(SourceId));
    public readonly BoxId TargetId = TargetId ?? throw new ArgumentNullException(nameof(TargetId));
}