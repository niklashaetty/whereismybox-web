using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task Execute(CreateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var user = new User(command.UserId, command.UserName, command.PrimaryCollectionId);
        await _userRepository.Create(user);
    }
}