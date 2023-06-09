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
        
        var user = new User(command.UserId, command.ExternalUserId, command.ExternalIdentityProvider, command.Username,
            command.PrimaryCollectionId);
        var userWithConflictingUsername = await _userRepository.SearchByUsername(command.Username);
        var userWithConflictingExternalUserId = await _userRepository.SearchByExternalUserId(command.ExternalUserId);

        if (userWithConflictingUsername is not null 
            || userWithConflictingExternalUserId is not null)
        {
            throw new UserAlreadyExistException("User already exist");
        }
        await _userRepository.Create(user);
    }
}