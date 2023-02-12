using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;
using Newtonsoft.Json;

namespace Infrastructure.UserRepository;

public class InMemoryUserRepository : IUserRepository
{
    private Dictionary<Guid, TestableUser> Users { get; } = new Dictionary<Guid, TestableUser>()
    {
        {
            Guid.Parse("deadbea7-deaf-d00d-c0de-1337da7aba5e"), 
            new TestableUser(new User(Guid.Parse("deadbea7-deaf-d00d-c0de-1337da7aba5e"), "Test user"))
        }
    };
    
    public Task<User> Create(User user)
    {
        var testableUser = new TestableUser(user);
        Users.Add(user.UserId, testableUser);
        return Task.FromResult(testableUser.Deserialize());
    }

    public Task<User> Get(Guid userId)
    {
        if (Users.ContainsKey(userId) is false)
        {
            throw new UserNotFoundException(userId);
        }

        return Task.FromResult(Users[userId].Deserialize());
    }

    public Task<User> PersistUpdate(User updatedUser)
    {
        if (Users.ContainsKey(updatedUser.UserId) is false)
        {
            throw new UserNotFoundException(updatedUser.UserId);
        }

        Users[updatedUser.UserId] = new TestableUser(updatedUser);
        return Task.FromResult(Users[updatedUser.UserId].Deserialize());
    }
}

internal class TestableUser
{
    public Guid UserId { get; }
    public string SerializedUser { get; }
    
    public TestableUser(User user)
    {
        UserId = user.UserId;
        SerializedUser = JsonConvert.SerializeObject(user);
    }

    public User Deserialize()
    {
        var result = JsonConvert.DeserializeObject<User>(SerializedUser);
        if (result is null)
        {
            throw new InvalidOperationException("Failed to deserialize into user");
        }

        return result;
    }
}