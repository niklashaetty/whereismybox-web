using Api;
using Api.Auth;
using Xunit;
using Xunit.Abstractions;

namespace NarrowIntegrationTests.Users;

public class UserTests
{
    private readonly Fixture _fixture;
    private readonly UserDriver _driver;

    public UserTests(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture(testOutputHelper);
        _driver = new UserDriver(_fixture);
    }

    [Fact]
    public async Task ShouldCreateUserWhenAssigningRolesForTheFirstTime()
    {
        // Given an user authenticated through an external idp for the first time
        var authenticatedExternalUser = new RolesRequest()
        {
            UserId = Guid.NewGuid().ToString(),
            IdentityProvider = "github",
            UserDetails = "Something"
        };

        // When assigning custom roles for authenticated user that does not exist
        var assignUserRolesResponse = await _driver.InvokeAssignUserRolesFunction(authenticatedExternalUser);

        // Then a role with a new userId should be returned
        var roleUserId = UserAssertions.AssertRoleWithUserIdReturned(assignUserRolesResponse);

        // When fetching the logged in user
        var getLoggedInUserResponse = await _driver.InvokeGetLoggedInUserFunction(authenticatedExternalUser.UserId);

        // Then the fetched user should be unregistered
        var currentUser = UserAssertions.AssertUnregisteredUser(getLoggedInUserResponse, expectedUserId:roleUserId);
        
        // When registering the user
        var registerUserResponse = await _driver.InvokeRegisterUserFunction(new RegisterUserRequest("Test username"), 
            currentUser.UserId);
        ResponseAssertions.AssertSuccessStatusCode(registerUserResponse);
        
        // And fetching it again
        getLoggedInUserResponse = await _driver.InvokeGetLoggedInUserFunction(authenticatedExternalUser.UserId);
        
        // Then the user should be registered
        UserAssertions.AssertRegisteredUser(getLoggedInUserResponse, expectedUserId:roleUserId);
    }
}