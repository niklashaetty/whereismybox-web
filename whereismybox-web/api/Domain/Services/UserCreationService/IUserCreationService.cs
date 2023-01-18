using Domain.Models;

namespace Domain.Services.UserCreationService;

public interface IUserCreationService
{
    public Task<User> Create(string userName);
}