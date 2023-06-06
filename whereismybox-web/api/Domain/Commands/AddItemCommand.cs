using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record AddItemCommand(ExternalUserId ExternalUserId, CollectionId CollectionId, BoxId BoxId, ItemId ItemId, string Name, string Description) : ICommand
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId  ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
    public readonly string Name = Name ?? throw new ArgumentNullException(nameof(Name));
    public readonly string Description = Description ?? throw new ArgumentNullException(nameof(Description));
 
}