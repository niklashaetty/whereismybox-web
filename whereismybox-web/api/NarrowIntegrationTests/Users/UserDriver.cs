using Api;
using Api.Auth;
using Functions.HttpTriggers.Users;
using Functions.HttpTriggers.V2;
using Microsoft.AspNetCore.Mvc;

namespace NarrowIntegrationTests.Users;

public class UserDriver
{
    private readonly Fixture _fixture;

    public UserDriver(Fixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }
    public async Task<IActionResult> InvokeAssignUserRolesFunction(RolesRequest request)
    {
        var function = new AssignUserRolesFunction(_fixture.GetUserByExternalUserIdQueryHandler,
            _fixture.CreateUserCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);
        httpRequest.AddExternallyAuthenticatedHeader(request.UserId);

        return await function.RunAsync(httpRequest);
    }
    
    public async Task<IActionResult> InvokeGetLoggedInUserFunction(string externalUserId)
    {
        var function = new GetLoggedInUserFunction(_fixture.GetUserByExternalUserIdQueryHandler,
            _fixture.CreateUserCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        httpRequest.AddExternallyAuthenticatedHeader(externalUserId);

        return await function.RunAsync(httpRequest);
    } 
    
    public async Task<IActionResult> InvokeRegisterUserFunction(RegisterUserRequest request, Guid userId)
    {
        var function = new RegisterUserFunction(_fixture.RegisterUserCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);
        httpRequest.AddFullyAuthenticatedUser(externalUserId:Guid.NewGuid().ToString(), userId.ToString());

        return await function.RunAsync(httpRequest);
    } 
}