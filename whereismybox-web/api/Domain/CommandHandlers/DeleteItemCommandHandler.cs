using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.CommandHandlers;

public class DeleteItemCommandHandler : ICommandHandler<DeleteItemCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly ILogger _logger;

    public DeleteItemCommandHandler(IAuthorizationService authorization, ILoggerFactory loggerFactory, IBoxRepository boxRepository,
        IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        _authorization = authorization;
        _logger = loggerFactory.CreateLogger<DeleteItemCommandHandler>();
        _boxRepository = boxRepository;
        _unattachedItemRepository = unattachedItemRepository;
    }

    public async Task Execute(DeleteItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.UserId, command.CollectionId);
        
        var box = await _boxRepository.Get(command.CollectionId, command.BoxId);

        if (box.TryGetItem(command.ItemId, out var item) is false)
        {
            _logger.LogInformation("Tried to remove non-existing item {ItemId}", command.ItemId);
            return;
        }

        if (command.IsHardDelete is false)
        {
            var unattachedItem = new UnattachedItem(command.CollectionId, item.ItemId, item.Name, item.Description,
                command.BoxId);
            await _unattachedItemRepository.Create(unattachedItem);
            _logger.LogInformation("Added {ItemId} to unattached items in collection {CollectionId}", command.ItemId,
                command.CollectionId);
        }

        if (box.RemoveItem(command.ItemId))
        {
            await _boxRepository.PersistUpdate(box);
            _logger.LogInformation("Removed {ItemId} from box {BoxId} in collection {CollectionId}", command.ItemId,
                command.BoxId,
                command.CollectionId);
        }
    }
}