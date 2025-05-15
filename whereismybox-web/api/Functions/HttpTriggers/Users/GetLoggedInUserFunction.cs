using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.QueryHandlers;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.Users;

public class GetLoggedInUserFunction
{
    private const string OperationId = "GetLoggedInUser";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<CreateUserCommand> _commandHandler;
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _queryHandler;

    public GetLoggedInUserFunction(IQueryHandler<GetUserByExternalUserIdQuery, User> queryHandler,
        ICommandHandler<CreateUserCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        ArgumentNullException.ThrowIfNull(commandHandler);
        _queryHandler = queryHandler;
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Users"},
        Summary =
            "Get the current logged in user by its external user. If the user does not exist, will create the user first internally")]
    [OpenApiParameter("primaryCollectionId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not logged in")]
    [Function(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/me")]
        HttpRequest req)
    {
        try
        {
            var externalUser = req.ParseExternalUser();

            var user = await _queryHandler.Handle(new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));
            return new OkObjectResult(user.ToApiModel());
        }
        catch (UserNotFoundException)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "User not found"));
        }
        catch (UnparsableExternalUserException e)
        {
            return new UnauthorizedResult();
        }
    }
}