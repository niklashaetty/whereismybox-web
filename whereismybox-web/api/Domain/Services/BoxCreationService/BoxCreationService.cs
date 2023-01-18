using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.BoxCreationService;

public class BoxCreationService : IBoxCreationService
{
    private readonly IBoxRepository _boxRepository;
    private readonly IUserRepository _userRepository;

    public BoxCreationService(IBoxRepository boxRepository, IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        ArgumentNullException.ThrowIfNull(userRepository);
        _boxRepository = boxRepository;
        _userRepository = userRepository;
    }

    public async Task<Box> Create(Guid userId, string boxName, int boxNumber)
    {
        var existingUser = await _userRepository.Get(userId);
        
        var boxesForUser = await _boxRepository.ListBoxesByUser(existingUser.UserId);
        if (boxesForUser.Any(b => b.Number == boxNumber))
        {
            throw new NonUniqueBoxException($"User already have a box with number {boxNumber}");
        }
        var box = Box.Create(boxNumber, boxName);
        return await _boxRepository.Add(existingUser.UserId, box);
    }
}