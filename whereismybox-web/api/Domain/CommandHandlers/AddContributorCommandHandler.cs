using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class AddContributorCommandHandler : ICommandHandler<AddContributorCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICollectionRepository _collectionRepository;

    public AddContributorCommandHandler(IUserRepository userRepository, ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _userRepository = userRepository;
        _collectionRepository = collectionRepository;
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

        var collection = await _collectionRepository.Get(command.CollectionId);
        collection.AddContributor(newContributor);
        await _collectionRepository.PersistUpdate(collection);
    }
}