using Api;
using Api.Auth;
using NarrowIntegrationTests.Users;
using Xunit.Abstractions;

namespace NarrowIntegrationTests;

public class TestDriver
{
    private readonly UserDriver _driver;

    public TestDriver(Fixture fixture)
    {
        _driver = new UserDriver(fixture);
    }
    
    public async Task<UserDto> GivenARegisteredUser()
    {
        var authenticatedExternalUser = new RolesRequest()
        {
            UserId = Guid.NewGuid().ToString(),
            IdentityProvider = "github",
            UserDetails = "Something"
        };
        
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
        
        // return it.
        return getLoggedInUserResponse.GetContentOfType<UserDto>();
    }
}