using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class CreateUserV2Function
{
    private const string OperationId = "CreateUserV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<CreateUserCommand> _commandHandler;

    public CreateUserV2Function(ICommandHandler<CreateUserCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Creates a new user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateUserRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")]
        HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var createUserRequest = JsonConvert.DeserializeObject<CreateUserRequest>(body);

        var userId = new UserId();
        var primaryCollectionId = CollectionId.GenerateNew();

        var command = new CreateUserCommand(userId, createUserRequest.UserName, primaryCollectionId);
        await _commandHandler.Execute(command);
        return new CreatedResult($"/api/users/{command.UserId}",
            new UserDto(command.UserId.Value, command.UserName, command.PrimaryCollectionId.Value));
    }
}