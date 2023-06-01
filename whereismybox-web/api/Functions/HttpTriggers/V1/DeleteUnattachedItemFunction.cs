using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Services.ItemDeletionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V1;

public class DeleteUnattachedItemFunction
{
    private const string OperationId = "DeleteUnattachedItem";
    private const string FunctionName = OperationId + "Function";
    private readonly IItemDeletionService _itemDeletionService;

    public DeleteUnattachedItemFunction(IItemDeletionService itemDeletionService)
    {
        ArgumentNullException.ThrowIfNull(itemDeletionService);
        _itemDeletionService = itemDeletionService;
    }

    [OpenApiOperation(operationId:OperationId, tags: new[] {"Items"}, Summary = "Delete an unattached item")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{userId}/items/{itemId}")]
        HttpRequest req,
        Guid userId,
        Guid itemId,
        ILogger log)
    {
        log.LogInformation("Remove an unattached item {ItemId} for user {UserId}", itemId, userId);
        await _itemDeletionService.DeleteUnattachedItem(userId, itemId);
        return new NoContentResult();

    }
}