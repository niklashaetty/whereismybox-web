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
        
        var source = await _boxRepository.Get(command.CollectionId, command.SourceId);
        
        if (source.TryGetItem(command.ItemId, out var item))
        {
            var target = await _boxRepository.Get(command.CollectionId, command.TargetId);
            target.AddItem(item);
            source.RemoveItem(item.ItemId);
            
            await _boxRepository.PersistUpdate(target);
            await _boxRepository.PersistUpdate(source);
        }
    }
}