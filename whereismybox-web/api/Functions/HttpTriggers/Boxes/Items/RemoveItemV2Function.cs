using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.Boxes.Items;

public class RemoveItemV2Function
{
    private const string OperationId = "RemoveItemV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<DeleteItemCommand> _deleteItemCommandHandler;

    public RemoveItemV2Function(ICommandHandler<DeleteItemCommand> deleteItemCommandHandler)
    {
        ArgumentNullException.ThrowIfNull(deleteItemCommandHandler);
        _deleteItemCommandHandler = deleteItemCommandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Items"}, Summary = "Remove item from box")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("hardDelete", In = ParameterLocation.Query, Required = false, Type = typeof(bool),
        Summary = "Indicates if an item should be removed from a box or hard deleted. Defaults to false " +
                  "(item is just removed from box).")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [Function(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
            Route = "collections/{collectionId}/boxes/{boxId}/items/{itemId}")]
        HttpRequest req,
        string collectionId,
        Guid boxId,
        Guid itemId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
            return new BadRequestObjectResult(
                new ErrorResponse("Validation error", "Invalid collectionId"));
        if (bool.TryParse(req.Query["hardDelete"], out var isHardDelete) is false) isHardDelete = false;

        try
        {
            var command = new DeleteItemCommand(req.ParseUserId(), domainCollectionId, new BoxId(boxId),
                new ItemId(itemId), isHardDelete);
            await _deleteItemCommandHandler.Execute(command);
            return new NoContentResult();
        }
        catch (BoxNotFoundException e)
        {
            // Fine, item still gone 
            return new NoContentResult();
        }
        catch (UnparsableExternalUserException e)
        {
            return new UnauthorizedResult();
        }
        catch (ForbiddenCollectionAccessException)
        {
            return new StatusCodeResult(403);
        }
    }
}