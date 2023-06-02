using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class CreateBoxCommandHandler : ICommandHandler<CreateBoxCommand>
{
    private readonly IBoxRepository _boxRepository;

    public CreateBoxCommandHandler(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    public async Task Execute(CreateBoxCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var existingBoxes = await _boxRepository.GetCollection(command.CollectionId);
        if (existingBoxes.Any(b => b.Number == command.BoxNumber))
        {
            throw new NonUniqueBoxException($"Collection already have a box with number {command.BoxNumber}");
        }

        var newBox = Box.Create(command.CollectionId, command.BoxId, command.BoxNumber, command.BoxName);
        await _boxRepository.Add(newBox);
    }
}