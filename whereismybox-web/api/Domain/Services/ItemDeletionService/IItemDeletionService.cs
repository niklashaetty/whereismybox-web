namespace Domain.Services.ItemDeletionService;

public interface IItemDeletionService
{
    public Task DeleteItem(Guid userId, Guid boxId, Guid itemId);
}