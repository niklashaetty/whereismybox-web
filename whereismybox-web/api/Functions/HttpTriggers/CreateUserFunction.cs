using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Services.UserCreationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.HttpTriggers;

public class CreateUserFunction
{
    private const string OperationId = "CreateUser";
    private const string FunctionName = OperationId + "Function";
    private readonly IUserCreationService _userCreationService;

    public CreateUserFunction(IUserCreationService userCreationService)
    {
        ArgumentNullException.ThrowIfNull(userCreationService);
        _userCreationService = userCreationService;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Creates a new user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateUserRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] 
        HttpRequest req, 
        ILogger log)
    {
        log.LogInformation("Creating a new user");
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var createUserRequest = JsonConvert.DeserializeObject<CreateUserRequest>(body);
        
        var newUser = await _userCreationService.Create(createUserRequest.UserName);
        return new CreatedResult($"/api/users/{newUser.UserId}", new UserDto(newUser.UserId, newUser.UserName));
    }
}