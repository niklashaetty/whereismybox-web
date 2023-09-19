using Domain.CommandHandlers;
using Domain.Models;
using Domain.Primitives;

namespace Domain.Commands;

public record CreateCollectionCommand(UserId Owner, CollectionId CollectionId, string Name) : ICommand
{
    public readonly UserId Owner = Owner ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly string Name = Name ?? throw new ArgumentNullException(nameof(Name));
    
}