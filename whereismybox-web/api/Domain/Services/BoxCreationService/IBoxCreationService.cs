using Domain.Models;

namespace Domain.Services.BoxCreationService;

public interface IBoxCreationService
{
    public Task<Box> Create(Guid userId, string boxName, int boxNumber);
}