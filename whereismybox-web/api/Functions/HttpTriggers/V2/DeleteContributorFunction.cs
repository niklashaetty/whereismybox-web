using System;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class DeleteContributorFunction
{
    private const string OperationId = "DeleteContributor";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<DeleteContributorCommand> _commandHandler;
    private readonly IQueryHandler<GetUserPermissionsQuery, Permissions> _permissionsQueryHandler;

    public DeleteContributorFunction(IQueryHandler<GetUserPermissionsQuery, Permissions> permissionsQueryHandler, ICommandHandler<DeleteContributorCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(permissionsQueryHandler);
        ArgumentNullException.ThrowIfNull(commandHandler);
        _permissionsQueryHandler = permissionsQueryHandler;
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Contributors"}, Summary = "Revoke access for another user to edit a collection")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(AddContributorRequest))]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "collections/{collectionId}/contributors/{userId}")]
        HttpRequest req,
        string collectionId,
        Guid userId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
        {
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
        }

        try
        {
            var userIdFromToken = req.ParseUserId();
            var permissions = await _permissionsQueryHandler.Handle(new GetUserPermissionsQuery(userIdFromToken));
            
            var command = new DeleteContributorCommand(permissions, new UserId(userId), domainCollectionId);
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