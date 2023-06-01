using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Domain.Services.BoxCreationService;

namespace Domain.CommandHandlers;

public class DeleteBoxCommandHandler : ICommandHandler<DeleteBoxCommand>
{
    private readonly IBoxRepository _boxRepository;

    public DeleteBoxCommandHandler(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    public async Task Execute(DeleteBoxCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _boxRepository.Delete(command.CollectionId, command.BoxId);
    }
}