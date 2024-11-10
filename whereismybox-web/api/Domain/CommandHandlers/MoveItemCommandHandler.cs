using Domain.Authorization;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.CommandHandlers;

public class MoveItemCommandHandler : ICommandHandler<MoveItemCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;
    private readonly ILogger _logger;

    public MoveItemCommandHandler(IAuthorizationService authorization,ILoggerFactory loggerFactory, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _authorization = authorization;
        _logger = loggerFactory.CreateLogger<MoveItemCommandHandler>();
        _boxRepository = boxRepository;
    }

    public async Task Execute(MoveItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.UserId, command.CollectionId);
        
        var existingBoxes = await _boxRepository.GetCollection(command.CollectionId);
        var source = existingBoxes.Find(box => box.BoxId == command.SourceId);
        var target = existingBoxes.Find(box => box.Number == command.TargetBoxNumber);
        
        if (source is null)
        {
            throw new BoxNotFoundException(command.CollectionId, command.SourceId);
        }

        if (target is null)
        {
            throw new BoxWithNumberNotFoundException(command.CollectionId, command.TargetBoxNumber);
        }
        
        if (source.TryGetItem(command.ItemId, out var item))
        {
            target.AddItem(item);
            source.RemoveItem(item.ItemId);
            
            await _boxRepository.PersistUpdate(target);
            await _boxRepository.PersistUpdate(source);
        }
    }
}