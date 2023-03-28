using Domain.Models;

namespace Domain.Services.UnattachedItemFetchingService;

public interface IUnattachedItemFetchingService
{
    public Task<UnattachedItemCollection> Get(Guid userId);
}