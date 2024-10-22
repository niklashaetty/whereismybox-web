using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Primitives;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class UpdateBoxCommandHandler : ICommandHandler<UpdateBoxCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;

    public UpdateBoxCommandHandler(IAuthorizationService authorization, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(authorization);
        _boxRepository = boxRepository;
        _authorization = authorization;
    }

    public async Task Execute(UpdateBoxCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.ExternalUserId, command.CollectionId);
        
        var box = await _boxRepository.Get(command.CollectionId, command.BoxId);
        if (command.BoxNumber != null)
        {
            if (await BoxNumberExists(command.CollectionId, command.BoxNumber.Value))
            {
                throw new NonUniqueBoxException($"Collection already have a box with number {command.BoxNumber}");
            }

            box.Number = command.BoxNumber.Value;
        }

        if (command.BoxName != null)
        {
            box.Name = command.BoxName;
        }
        
        await _boxRepository.PersistUpdate(box);
    }

    private async Task<bool> BoxNumberExists(CollectionId collectionId, int boxNumber)
    {
        var existingBoxes = await _boxRepository.GetCollection(collectionId);
        return existingBoxes.Any(b => b.Number == boxNumber);
    }
}