using Domain.Commands;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task Execute(RegisterUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var user = await _userRepository.Get(command.UserId);
        user.RegisterUsername(command.Username);
        await _userRepository.PersistUpdate(user);
    }
}