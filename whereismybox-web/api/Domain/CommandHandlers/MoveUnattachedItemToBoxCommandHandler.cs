using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.CommandHandlers;

public class MoveUnattachedItemToCommandHandler : ICommandHandler<MoveUnattachedItemToBoxCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly ILogger _logger;

    public MoveUnattachedItemToCommandHandler(IAuthorizationService authorization, ILoggerFactory loggerFactory,
        IBoxRepository boxRepository,
        IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        _authorization = authorization;
        _logger = loggerFactory.CreateLogger<MoveUnattachedItemToCommandHandler>();
        _boxRepository = boxRepository;
        _unattachedItemRepository = unattachedItemRepository;
    }

    public async Task Execute(MoveUnattachedItemToBoxCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.UserId, command.CollectionId);

        var unattachedItems = await _unattachedItemRepository.GetCollection(command.CollectionId);
        var unattachedItem = unattachedItems.FirstOrDefault(i => i.ItemId == command.ItemId);
        if (unattachedItem is null)
        {
            throw new ItemNotFoundException(command.CollectionId, command.ItemId);
        }

        var boxes = await _boxRepository.GetCollection(command.CollectionId);
        var box = boxes.FirstOrDefault(box => box.Number == command.BoxNumber);

        if (box is null)
        {
            throw new BoxWithNumberNotFoundException(command.CollectionId, command.BoxNumber);
        }

        box.AddItem(unattachedItem);
        await _boxRepository.PersistUpdate(box);

        await _unattachedItemRepository.Delete(command.CollectionId, unattachedItem.ItemId);
    }
}