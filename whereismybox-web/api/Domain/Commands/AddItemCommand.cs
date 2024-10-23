using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record AddItemCommand(UserId UserId, CollectionId CollectionId, BoxId BoxId, ItemId ItemId, string Name, string Description) : ICommand
{
    public UserId UserId { get; } = UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
    public readonly ItemId ItemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
    public readonly string Name = Name ?? throw new ArgumentNullException(nameof(Name));
    public readonly string Description = Description ?? throw new ArgumentNullException(nameof(Description));
 
}