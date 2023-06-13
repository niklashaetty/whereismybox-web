using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class DeleteContributorCommandHandler : ICommandHandler<DeleteContributorCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteContributorCommandHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task Execute(DeleteContributorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Permissions.IsOwner(command.Permissions.UserId, command.CollectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }

        var userToBeRemoved = await _userRepository.Get(command.UserToBeRemoved);
        if (userToBeRemoved is null)
        {
            return;
        }
        
        userToBeRemoved.RemoveAsContributor(command.CollectionId);
        await _userRepository.PersistUpdate(userToBeRemoved);
    }
}