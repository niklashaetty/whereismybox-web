using Domain.Authorization;
using Domain.Commands;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class AddItemCommandHandler : ICommandHandler<AddItemCommand>
{
    private readonly IAuthorizationService _authorization;
    private readonly IBoxRepository _boxRepository;

    public AddItemCommandHandler(IAuthorizationService authorization, IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(boxRepository);
        _authorization = authorization;
        _boxRepository = boxRepository;
    }

    public async Task Execute(AddItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        await _authorization.EnsureCollectionAccessAllowed(command.UserId, command.CollectionId);
        
        var newItem = new Item(command.ItemId, command.Name, command.Description);
        
        var existingBox = await _boxRepository.Get(command.CollectionId, command.BoxId);
        existingBox.AddItem(newItem);
        await _boxRepository.PersistUpdate(existingBox);
    }
}