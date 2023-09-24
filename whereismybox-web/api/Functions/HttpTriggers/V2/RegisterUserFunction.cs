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
using Domain.Queries;
using Domain.QueryHandlers;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class RegisterUserFunction
{
    private const string OperationId = "RegisterUsername";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<RegisterUserCommand> _commandHandler;

    public RegisterUserFunction(ICommandHandler<RegisterUserCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"},
        Summary = "Register a user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(RegisterUserRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/register")]
        HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var registerUserRequest = JsonConvert.DeserializeObject<RegisterUserRequest>(body);
            var userId = req.ParseUserId();
            var command = new RegisterUserCommand(userId, registerUserRequest.UserName);

            await _commandHandler.Execute(command);
            return new NoContentResult();
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