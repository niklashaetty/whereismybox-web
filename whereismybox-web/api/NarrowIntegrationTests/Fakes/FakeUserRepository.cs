using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.UserRepository;
using Newtonsoft.Json;

namespace NarrowIntegrationTests.Fakes;

public class FakeUserRepository : IUserRepository
{
    private List<TestableUser> _database = new ();
    public Task<User> Create(User user)
    {
        var cosmosAwareUser = CosmosAwareUser.ToCosmosAware(user);
        cosmosAwareUser.ETag = Guid.NewGuid().ToString();
        var testableUser = new TestableUser(cosmosAwareUser);
        
        return Task.FromResult<User>(testableUser.AsCosmosAware());
    }

    public Task<User> Get(UserId userId)
    {
        var testableUser = _database.SingleOrDefault(u => u.UserId == userId);
        if (testableUser is null)
        {
            throw new UserNotFoundException(userId);
        }

        return Task.FromResult<User>(testableUser.AsCosmosAware());
    }

    public Task<User> PersistUpdate(User updatedUser)
    {
        throw new NotImplementedException();
    }
}

public class TestableUser
{
    public UserId UserId { get; set; }
    public string SerializedCosmosAwareUser { get; set; }

    public TestableUser(CosmosAwareUser cosmosAwareUser)
    {
        ArgumentNullException.ThrowIfNull(cosmosAwareUser);
        UserId = cosmosAwareUser.UserId;
        SerializedCosmosAwareUser = JsonConvert.SerializeObject(cosmosAwareUser);
    }

    public CosmosAwareUser AsCosmosAware()
    {
        return JsonConvert.DeserializeObject<CosmosAwareUser>(SerializedCosmosAwareUser) ??
               throw new InvalidOperationException("Test error");
    }
}