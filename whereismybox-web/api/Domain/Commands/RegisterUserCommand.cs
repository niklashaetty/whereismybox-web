using Domain.CommandHandlers;
using Domain.Primitives;

namespace Domain.Commands;

public record RegisterUserCommand(UserId UserId, string Username) : ICommand
{
    public readonly UserId UserId = UserId ?? throw new ArgumentNullException(nameof(UserId));
    public readonly string Username = Username ?? throw new ArgumentNullException(nameof(Username));
}