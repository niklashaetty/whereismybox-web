using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record UpdateBoxCommand(UserId UserId, CollectionId CollectionId, BoxId BoxId, int? BoxNumber, string BoxName) : ICommand
{
    public readonly UserId UserId = UserId ?? throw new ArgumentNullException(nameof(UserId));
    public readonly CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public readonly BoxId BoxId = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
    public readonly int? BoxNumber = BoxNumber;
    public readonly string? BoxName = BoxName ?? throw new ArgumentNullException(nameof(BoxName));
    
}