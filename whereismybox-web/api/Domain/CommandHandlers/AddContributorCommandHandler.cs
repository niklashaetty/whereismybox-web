using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class AddContributorCommandHandler : ICommandHandler<AddContributorCommand>
{
    private readonly IUserRepository _userRepository;

    public AddContributorCommandHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task Execute(AddContributorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Permissions.IsOwner(command.OwnerOfCollection, command.CollectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }

        var newContributor = await _userRepository.SearchByUsername(command.UsernameToBeAddedAsContributor);
        if (newContributor is null)
        {
            throw new UserNotFoundException($"User with username {command.UsernameToBeAddedAsContributor}");
        }
        
        newContributor.AddAsContributor(command.CollectionId);
        await _userRepository.PersistUpdate(newContributor);
    }
}