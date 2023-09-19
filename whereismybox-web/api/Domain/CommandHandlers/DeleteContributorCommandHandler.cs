using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class DeleteContributorCommandHandler : ICommandHandler<DeleteContributorCommand>
{
    private readonly ICollectionRepository _collectionRepository;

    public DeleteContributorCommandHandler(ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _collectionRepository = collectionRepository;
    }

    public async Task Execute(DeleteContributorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Permissions.IsOwner(command.Permissions.UserId, command.CollectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }

        var collection = await _collectionRepository.Get(command.CollectionId);
        collection.RemoveAsContributor(command.UserToBeRemoved);
        await _collectionRepository.PersistUpdate(collection);
    }
}