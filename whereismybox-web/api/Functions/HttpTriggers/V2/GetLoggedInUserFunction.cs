using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Api;
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
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class GetLoggedInUserFunction
{
    private const string OperationId = "GetLoggedInUser";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _queryHandler;
    private readonly ICommandHandler<CreateUserCommand> _commandHandler;

    public GetLoggedInUserFunction(IQueryHandler<GetUserByExternalUserIdQuery, User> queryHandler,
        ICommandHandler<CreateUserCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        ArgumentNullException.ThrowIfNull(commandHandler);
        _queryHandler = queryHandler;
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"},
        Summary =
            "Get the current logged in user by its external user. If the user does not exist, will create the user first internally")]
    [OpenApiParameter("primaryCollectionId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not logged in")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/me")]
        HttpRequest req,
        ILogger log)
    {
        try
        {
            var externalUser = req.ParseExternalUser();

            var user = await _queryHandler.Handle(new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));
            return new OkObjectResult(user.ToApiModel());
        }
        catch (UserNotFoundException)
        {
            var externalUser = req.ParseExternalUser();
            log.LogInformation("User with externalId {ExternalUserId} not found, will create a new",
                externalUser.ExternalUserId);
            
            var newUser = new User(new UserId(), externalUser.ExternalUserId, externalUser.ExternalIdentityProvider,
                externalUser.Username, CollectionId.GenerateNew());
            await _commandHandler.Execute(new CreateUserCommand(newUser.UserId, newUser.ExternalUserId,
                newUser.ExternalIdentityProvider, newUser.Username, newUser.PrimaryCollectionId));
            
            log.LogInformation("Created new user {UserId} with primaryCollectionId {PrimaryCollectionId}",
                newUser.UserId, newUser.PrimaryCollectionId);
            return new OkObjectResult(newUser.ToApiModel());
        }
        catch (UnparsableExternalUserException e)
        {
            return new UnauthorizedResult();
        }
    }
}