using Domain.Commands;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Domain.CommandHandlers;

public class DeleteUnattachedItemCommandHandler : ICommandHandler<DeleteUnattachedItemCommand>
{
    private readonly IUnattachedItemRepository _unattachedItemRepository;
    private readonly ILogger _logger;

    public DeleteUnattachedItemCommandHandler(ILoggerFactory loggerFactory,
        IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        _logger = loggerFactory.CreateLogger<DeleteItemCommandHandler>();
        _unattachedItemRepository = unattachedItemRepository;
    }

    public async Task Execute(DeleteUnattachedItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _unattachedItemRepository.Delete(command.CollectionId, command.ItemId);
    }
}