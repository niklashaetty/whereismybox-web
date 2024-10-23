using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class CreateBoxCommandHandler : ICommandHandler<CreateBoxCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;

    public CreateBoxCommandHandler(IAuthorizationService authorization, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(authorization);
        _boxRepository = boxRepository;
        _authorization = authorization;
    }

    public async Task Execute(CreateBoxCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.UserId, command.CollectionId);
        
        var existingBoxes = await _boxRepository.GetCollection(command.CollectionId);
        if (existingBoxes.Any(b => b.Number == command.BoxNumber))
        {
            throw new NonUniqueBoxException($"Collection already have a box with number {command.BoxNumber}");
        }

        var newBox = Box.Create(command.CollectionId, command.BoxId, command.BoxNumber, command.BoxName);
        await _boxRepository.Add(newBox);
    }
}