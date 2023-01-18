using Domain.Models;
using Domain.Repositories;

namespace Domain.Services.UserCreationService;

public class UserCreationService : IUserCreationService
{
    private readonly IUserRepository _userRepository;

    public UserCreationService(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task<User> Create(string userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        
        var newUser = User.Create(userName);
        return await _userRepository.Create(newUser);
    }
}