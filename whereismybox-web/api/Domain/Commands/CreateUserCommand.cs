using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record CreateUserCommand(UserId UserId, ExternalUserId ExternalUserId, string ExternalIdentityProvider, string Username,
    CollectionId PrimaryCollectionId) : ICommand
{
    public readonly UserId UserId = UserId ?? throw new ArgumentNullException(nameof(UserId));
    public readonly ExternalUserId ExternalUserId = ExternalUserId ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public readonly string ExternalIdentityProvider = ExternalIdentityProvider ?? throw new ArgumentNullException(nameof(ExternalIdentityProvider));
    public readonly string Username = Username ?? throw new ArgumentNullException(nameof(Username));
    public CollectionId PrimaryCollectionId { get; } =
        PrimaryCollectionId ?? throw new ArgumentNullException(nameof(PrimaryCollectionId));
}