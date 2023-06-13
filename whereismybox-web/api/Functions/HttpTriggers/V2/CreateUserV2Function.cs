using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
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

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"},
        Summary = "Creates a new user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateUserRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequest req)
    {
        try
        {
            var externalUser = req.ParseExternalUser();
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var createUserRequest = JsonConvert.DeserializeObject<CreateUserRequest>(body);
            var newUser = new User(new UserId(), externalUser.ExternalUserId, externalUser.ExternalIdentityProvider,
                createUserRequest.UserName, CollectionId.GenerateNew(), new List<CollectionId>());
            var command = new CreateUserCommand(newUser.UserId, newUser.ExternalUserId,
                newUser.ExternalIdentityProvider, newUser.Username, newUser.PrimaryCollectionId);

            await _commandHandler.Execute(command);
            return new CreatedResult($"/api/users/{newUser.UserId}",
                newUser.ToApiModel());
        }
        catch (UserAlreadyExistException)
        {
            return new ConflictResult();
        }
        catch (UnparsableExternalUserException)
        {
            return new UnauthorizedResult();
        }
    }
}