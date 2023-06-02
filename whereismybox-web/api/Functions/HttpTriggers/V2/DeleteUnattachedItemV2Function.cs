using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Exceptions;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V2;

public class DeleteUnattachedItemV2Function
{
    private const string OperationId = "DeleteUnattachedItemV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<DeleteUnattachedItemCommand> _deleteUnattachedItemCommandHandler;

    public DeleteUnattachedItemV2Function(
        ICommandHandler<DeleteUnattachedItemCommand> deleteUnattachedItemCommandHandler)
    {
        ArgumentNullException.ThrowIfNull(deleteUnattachedItemCommandHandler);
        _deleteUnattachedItemCommandHandler = deleteUnattachedItemCommandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Unattached items"},
        Summary = "Remove an unattached item")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete",
            Route = "collections/{collectionId}/unattached-items/{itemId}")]
        HttpRequest req,
        string collectionId,
        Guid itemId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
        {
            return new BadRequestObjectResult(
                new ErrorResponse("Validation error", "Invalid collectionId"));
        }

        var command = new DeleteUnattachedItemCommand(domainCollectionId, new ItemId(itemId));
        await _deleteUnattachedItemCommandHandler.Execute(command);
        return new NoContentResult();
    }
}