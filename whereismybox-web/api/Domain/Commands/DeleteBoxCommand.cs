using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteBoxCommand(CollectionId CollectionId, BoxId BoxId) : ICommand
{
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
}