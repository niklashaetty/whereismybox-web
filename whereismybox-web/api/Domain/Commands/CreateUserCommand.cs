using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record CreateUserCommand(UserId UserId, string UserName, CollectionId PrimaryCollectionId) : ICommand
{
    public readonly UserId UserId = UserId ?? throw new ArgumentNullException(nameof(UserId));
    public readonly string UserName = UserName ?? throw new ArgumentNullException(nameof(UserName));
    public CollectionId PrimaryCollectionId { get; } =
        PrimaryCollectionId ?? throw new ArgumentNullException(nameof(PrimaryCollectionId));
}