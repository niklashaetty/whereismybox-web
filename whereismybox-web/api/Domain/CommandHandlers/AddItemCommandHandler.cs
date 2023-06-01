using Domain.Commands;
using Domain.Models;
using Domain.Repositories;

namespace Domain.CommandHandlers;

public class AddItemCommandHandler : ICommandHandler<AddItemCommand>
{
    private readonly IBoxRepository _boxRepository;

    public AddItemCommandHandler(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    public async Task Execute(AddItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var newItem = new Item(command.ItemId, command.Name, command.Description);
        
        var existingBox = await _boxRepository.Get(command.CollectionId, command.BoxId);
        existingBox.AddItem(newItem);
        await _boxRepository.PersistUpdate(existingBox);
    }
}