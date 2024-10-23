using System.Net;
using Domain.Exceptions;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.UserRepository;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using User = Domain.Models.User;

namespace NarrowIntegrationTests.Fakes.FakeUserRepository;

public class FakeUserRepository : IUserRepository
{
    private readonly List<TestableUser> _database = new();

    public Task<User> Create(User user)
    {
        var cosmosAwareUser = CosmosAwareUser.ToCosmosAware(user);
        cosmosAwareUser.ETag = Guid.NewGuid().ToString();
        var testableUser = new TestableUser(cosmosAwareUser);
        
        _database.Add(testableUser);
        return Task.FromResult<User>(testableUser.AsCosmosAware());
    }

    public Task<User?> Get(UserId userId)
    {
        var testableUser = _database.SingleOrDefault(u => u.UserId == userId);

        return Task.FromResult<User?>(testableUser?.AsCosmosAware());
    }

    public Task<User?> SearchByUsername(string username)
    {
        var testableUser = _database.FirstOrDefault(u => u.Username == username);
        return testableUser is null
            ? Task.FromResult<User?>(null)
            : Task.FromResult<User?>(testableUser.AsCosmosAware());
    }

    public Task<User?> SearchByExternalUserId(ExternalUserId externalUserId)
    {
        var testableUser = _database.FirstOrDefault(u => u.ExternalUserId.Equals(externalUserId));
        return testableUser is null
            ? Task.FromResult<User?>(null)
            : Task.FromResult<User?>(testableUser.AsCosmosAware());
    }

    public Task<User> PersistUpdate(User updatedUser)
    {

        if (updatedUser is not CosmosAwareUser newCosmosAwareUser)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var testableUser = _database.SingleOrDefault(u => u.UserId == updatedUser.UserId);
        if (testableUser is null)
        {
            throw new UserNotFoundException(updatedUser.UserId);
        }

        var oldCosmosAwareUser = testableUser.AsCosmosAware();
        if (newCosmosAwareUser.ETag != oldCosmosAwareUser.ETag)
        {
            throw new CosmosException("Opportunistic locking failed!", 
                HttpStatusCode.Conflict, 409, "1", 1.0);
        }

        _database.RemoveAll(u => u.UserId.Equals(updatedUser.UserId));
        newCosmosAwareUser.ETag = Guid.NewGuid().ToString();
        _database.Add(new TestableUser(newCosmosAwareUser));
        return Task.FromResult<User>(newCosmosAwareUser);
    }
}

public class TestableUser
{
    public UserId UserId { get; set; }
    public ExternalUserId ExternalUserId { get; set; }
    public string Username { get; set; }
    public string SerializedCosmosAwareUser { get; set; }

    public TestableUser(CosmosAwareUser cosmosAwareUser)
    {
        ArgumentNullException.ThrowIfNull(cosmosAwareUser);
        UserId = cosmosAwareUser.UserId;
        Username = cosmosAwareUser.Username;
        ExternalUserId = cosmosAwareUser.ExternalUserId;
        SerializedCosmosAwareUser = JsonConvert.SerializeObject(cosmosAwareUser);
    }

    public CosmosAwareUser AsCosmosAware()
    {
        return JsonConvert.DeserializeObject<CosmosAwareUser>(SerializedCosmosAwareUser) ??
               throw new InvalidOperationException("Test error");
    }
}