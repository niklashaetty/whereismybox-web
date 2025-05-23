using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.QueryHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.Collections;

public class DeleteCollectionFunction
{
    private const string OperationId = "DeleteCollection";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<DeleteCollectionCommand> _commandHandler;
    private readonly IQueryHandler<GetUserPermissionsQuery, Permissions> _permissions;

    public DeleteCollectionFunction(IQueryHandler<GetUserPermissionsQuery, Permissions> permissionsQueryHandler,
        ICommandHandler<DeleteCollectionCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(permissionsQueryHandler);
        ArgumentNullException.ThrowIfNull(commandHandler);
        _permissions = permissionsQueryHandler;
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Collections"}, Summary = "Soft delete a collection")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [Function(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "collections/{collectionId}")]
        HttpRequest req,
        string collectionId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));

        try
        {
            var userIdFromToken = req.ParseUserId();
            var permissions = await _permissions.Handle(new GetUserPermissionsQuery(userIdFromToken));

            var command = new DeleteCollectionCommand(permissions, domainCollectionId);
            await _commandHandler.Execute(command);

            return new NoContentResult();
        }
        catch (UnparsableExternalUserException)
        {
            return new UnauthorizedResult();
        }
        catch (ForbiddenCollectionAccessException)
        {
            return new StatusCodeResult(403);
        }
    }
}