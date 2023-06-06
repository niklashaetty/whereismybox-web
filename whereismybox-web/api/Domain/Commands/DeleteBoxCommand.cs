using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record DeleteBoxCommand(ExternalUserId ExternalUserId, CollectionId CollectionId, BoxId BoxId) : ICommand
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId  ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
}