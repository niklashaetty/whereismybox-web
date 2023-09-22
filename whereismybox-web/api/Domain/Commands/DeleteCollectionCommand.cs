using Domain.CommandHandlers;
using Domain.Models;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteCollectionCommand(Permissions Permissions, CollectionId CollectionId) : ICommand
{
    public readonly Permissions Permissions = Permissions ?? throw new ArgumentNullException(nameof(Permissions));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}