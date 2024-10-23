using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteBoxCommand(UserId UserId, CollectionId CollectionId, BoxId BoxId) : ICommand
{
    public UserId UserId { get; } = UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
}