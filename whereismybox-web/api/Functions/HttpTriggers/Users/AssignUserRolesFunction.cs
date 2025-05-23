using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Api.Auth;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.QueryHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.Users;

public class AssignUserRolesFunction
{
    private const string OperationId = "GetRoles";
    private readonly ICommandHandler<CreateUserCommand> _createUserCommandHandler;
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _getUserByExternalIdQueryHandler;

    public AssignUserRolesFunction(IQueryHandler<GetUserByExternalUserIdQuery, User> getUserByExternalIdQueryHandler,
        ICommandHandler<CreateUserCommand> createUserCommandHandler)
    {
        ArgumentNullException.ThrowIfNull(getUserByExternalIdQueryHandler);
        ArgumentNullException.ThrowIfNull(createUserCommandHandler);
        _getUserByExternalIdQueryHandler = getUserByExternalIdQueryHandler;
        _createUserCommandHandler = createUserCommandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Users"}, Summary = "Fetch the user object of the current logged in user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(RolesResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not found")]
    [Function("GetRoles")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetRoles")]
        HttpRequest req)
    {
        User user;
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var rolesRequest = JsonConvert.DeserializeObject<RolesRequest>(body);
        var externalUser = rolesRequest.AsExternalUser();
        try
        {
            user = await _getUserByExternalIdQueryHandler.Handle(
                new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));
        }
        catch (UserNotFoundException)
        {
            var temporaryUsername = externalUser.Username + "-" + CollectionId.GenerateNew();
            await _createUserCommandHandler.Execute(new CreateUserCommand(new UserId(), externalUser.ExternalUserId,
                externalUser.ExternalIdentityProvider, temporaryUsername));
            user = await _getUserByExternalIdQueryHandler.Handle(
                new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));
        }

        var response = user.AsRolesResponse();
        return new OkObjectResult(response);
    }
}