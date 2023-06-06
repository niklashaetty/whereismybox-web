using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class DeleteBoxCommandHandler : ICommandHandler<DeleteBoxCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;

    public DeleteBoxCommandHandler(IAuthorizationService authorization, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _authorization = authorization;
        _boxRepository = boxRepository;
    }

    public async Task Execute(DeleteBoxCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.ExternalUserId, command.CollectionId);
        
        await _boxRepository.Delete(command.CollectionId, command.BoxId);
    }
}