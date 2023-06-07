using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
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

namespace Functions.HttpTriggers.V2;

public class AssignUserRolesFunction
{
    private const string OperationId = "GetRoles";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _getUserByExternalIdQueryHandler;
    private readonly ICommandHandler<CreateUserCommand> _createUserCommandHandler;

    public AssignUserRolesFunction(IQueryHandler<GetUserByExternalUserIdQuery, User> getUserByExternalIdQueryHandler, ICommandHandler<CreateUserCommand> createUserCommandHandler)
    {
        ArgumentNullException.ThrowIfNull(getUserByExternalIdQueryHandler);
        ArgumentNullException.ThrowIfNull(createUserCommandHandler);
        _getUserByExternalIdQueryHandler = getUserByExternalIdQueryHandler;
        _createUserCommandHandler = createUserCommandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Fetch the user object of the current logged in user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(RolesResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not found")]
    [FunctionName("GetRoles")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetRoles")] 
        HttpRequest req,
        ILogger log)
    {
        try
        {
            var externalUser = req.ParseExternalUser();

            var user = await _getUserByExternalIdQueryHandler.Handle(new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));
            return new OkObjectResult(ParseRoles(user));
        }
        catch (UserNotFoundException)
        {
            var externalUser = req.ParseExternalUser();
            log.LogInformation("User with externalId {ExternalUserId} not found, will create a new",
                externalUser.ExternalUserId);
            
            var newUser = new User(new UserId(), externalUser.ExternalUserId, externalUser.ExternalIdentityProvider,
                externalUser.Username, CollectionId.GenerateNew());
            await _createUserCommandHandler.Execute(new CreateUserCommand(newUser.UserId, newUser.ExternalUserId,
                newUser.ExternalIdentityProvider, newUser.Username, newUser.PrimaryCollectionId));
            
            log.LogInformation("Created new user {UserId} with primaryCollectionId {PrimaryCollectionId}",
                newUser.UserId, newUser.PrimaryCollectionId);
            return new OkObjectResult(ParseRoles(newUser));
        }
        catch (UnparsableExternalUserException e)
        {
            return new UnauthorizedResult();
        }
    }

    private static RolesResponse ParseRoles(User user)
    {
        var roles = new List<string> {"userId:" + user.PrimaryCollectionId};
        return new RolesResponse(roles);
    }
}