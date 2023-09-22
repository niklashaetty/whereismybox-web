using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class DeleteCollectionCommandHandler : ICommandHandler<DeleteCollectionCommand>
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IBoxRepository _boxRepository;
    private const int DeletedTimeToLiveInDays = 30;

    public DeleteCollectionCommandHandler(ICollectionRepository collectionRepository, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _collectionRepository = collectionRepository;
        _boxRepository = boxRepository;
    }

    public async Task Execute(DeleteCollectionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.Permissions.IsOwner(command.Permissions.UserId, command.CollectionId) is false)
        {
            throw new ForbiddenCollectionAccessException();
        }

        var boxes = await _boxRepository.GetCollection(command.CollectionId);

        foreach (var box in boxes)
        {
            await _boxRepository.ScheduleForDeletion(command.CollectionId, box, DeletedTimeToLiveInDays);
        }

        await _collectionRepository.Delete(command.CollectionId);
    }
}