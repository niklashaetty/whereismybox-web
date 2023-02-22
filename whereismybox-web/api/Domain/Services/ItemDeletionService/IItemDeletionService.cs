namespace Domain.Services.ItemDeletionService;

public interface IItemDeletionService
{
    public Task DeleteItem(Guid userId, Guid boxId, Guid itemId, bool isHardDelete);
    public Task DeleteUnattachedItem(Guid userId, Guid itemId);
}