using Domain.Authorization;
using Domain.Commands;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.CommandHandlers;

public class DeleteUnattachedItemCommandHandler : ICommandHandler<DeleteUnattachedItemCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly ILogger _logger;

    public DeleteUnattachedItemCommandHandler(IAuthorizationService authorization, ILoggerFactory loggerFactory,
        IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        
        _authorization = authorization;
        _logger = loggerFactory.CreateLogger<DeleteItemCommandHandler>();
        _unattachedItemRepository = unattachedItemRepository;
    }

    public async Task Execute(DeleteUnattachedItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.ExternalUserId, command.CollectionId);
        
        await _unattachedItemRepository.Delete(command.CollectionId, command.ItemId);
    }
}