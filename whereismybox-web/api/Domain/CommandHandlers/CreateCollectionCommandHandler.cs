using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class CreateCollectionCommandHandler : ICommandHandler<CreateCollectionCommand>
{
    private readonly ICollectionRepository _collectionRepository;

    public CreateCollectionCommandHandler(ICollectionRepository collectionRepository)
    {
        ArgumentNullException.ThrowIfNull(collectionRepository);
        _collectionRepository = collectionRepository;
    }

    public async Task Execute(CreateCollectionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var newCollection = Collection.Create(command.CollectionId, command.Name, command.Owner);
        await _collectionRepository.Create(newCollection);
    }
}