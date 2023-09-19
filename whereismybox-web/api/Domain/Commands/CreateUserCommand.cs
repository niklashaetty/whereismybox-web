using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record CreateUserCommand(UserId UserId, ExternalUserId ExternalUserId, string ExternalIdentityProvider, string Username) : ICommand
{
    public readonly UserId UserId = UserId ?? throw new ArgumentNullException(nameof(UserId));
    public ExternalUserId ExternalUserId { get; }= ExternalUserId ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public readonly string ExternalIdentityProvider = ExternalIdentityProvider ?? throw new ArgumentNullException(nameof(ExternalIdentityProvider));
    public readonly string Username = Username ?? throw new ArgumentNullException(nameof(Username));
}