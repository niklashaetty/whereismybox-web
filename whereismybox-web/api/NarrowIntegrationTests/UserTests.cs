using Api;
using Xunit;
using Xunit.Abstractions;

namespace NarrowIntegrationTests;

public class UserTests
{
    private readonly Fixture _fixture;
    private readonly Driver _driver;

    public UserTests(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture(testOutputHelper);
        _driver = new Driver(_fixture);
    }

    [Fact]
    public async void GetUserByCollectionId()
    {
        // When creating a user
        var userName = "Some username";
        var createUserResponse = await _driver.InvokeCreateUserFunction(new CreateUserRequest(userName));

        // Then there should be a user
        ResponseAssertions.AssertSuccessStatusCode(createUserResponse);
        var firstUser = createUserResponse.GetContentOfType<UserDto>();
        var collectionId = firstUser.PrimaryCollectionId;
        
        // When getting the user by collectionId
        var getUserResponse = await _driver.InvokeGetUserByCollectionIdFunction(collectionId);
        ResponseAssertions.AssertSuccessStatusCode(createUserResponse);
        var secondUser = createUserResponse.GetContentOfType<UserDto>();
        
        // Assert that we find the user
        Assert.Equal(firstUser.UserId, secondUser.UserId);
        Assert.Equal(firstUser.PrimaryCollectionId, secondUser.PrimaryCollectionId);
        Assert.Equal(firstUser.Username, secondUser.Username);
    }
}