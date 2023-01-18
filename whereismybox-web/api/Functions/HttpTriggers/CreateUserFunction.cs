using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain;
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
    private readonly IUserCreationService _userCreationService;

    public CreateUserFunction(IUserCreationService userCreationService)
    {
        ArgumentNullException.ThrowIfNull(userCreationService);
        _userCreationService = userCreationService;
    }

    [OpenApiOperation(operationId: "CreateUser", tags: new[] {"Quiz"}, Summary = "Creates a new user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateUserRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName("CreateUserFunction")]
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