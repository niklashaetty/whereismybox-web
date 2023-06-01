using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Exceptions;
using Domain.Repositories;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace Functions.HttpTriggers.V1;

public class GetUserFunction
{
    private const string OperationId = "GetUser";
    private const string FunctionName = OperationId + "Function";
    private readonly IUserRepository _userRepository;

    public GetUserFunction(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Get an existing user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{userId}")] 
        HttpRequest req, 
        ILogger log,
        Guid userId)
    {
        log.LogInformation("Fetching user {UserId}", userId);
        try
        {
            var user = await _userRepository.Get(userId);
            return new OkObjectResult(user.ToApiModel());
        }
        catch (UserNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "User was not found"));
        }
    }
}