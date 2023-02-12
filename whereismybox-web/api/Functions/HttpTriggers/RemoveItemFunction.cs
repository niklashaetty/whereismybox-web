using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Exceptions;
using Domain.Services.ItemDeletionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers;

public class RemoveItemFunction
{
    private const string OperationId = "RemoveItem";
    private const string FunctionName = OperationId + "Function";
    private readonly IItemDeletionService _itemDeletionService;

    public RemoveItemFunction(IItemDeletionService itemDeletionService)
    {
        ArgumentNullException.ThrowIfNull(itemDeletionService);
        _itemDeletionService = itemDeletionService;
    }

    [OpenApiOperation(operationId:OperationId, tags: new[] {"Items"}, Summary = "Remove item from box")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("hardDelete", In = ParameterLocation.Query, Required = false, Type = typeof(bool), 
        Summary = "Indicates if an item should be removed from a box or hard deleted. Defaults to false " +
                  "(item is just removed from box).")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{userId}/boxes/{boxId}/items/{itemId}")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        Guid itemId,
        ILogger log)
    {
        log.LogInformation("Remove a item {ItemId} for user {UserId} and box {BoxId}", itemId, userId, boxId);

        if (bool.TryParse(req.Query["hardDelete"], out var isHardDelete) is false)
        {
            isHardDelete = false;
        }

        try
        {
            await _itemDeletionService.DeleteItem(userId, boxId, itemId, isHardDelete);
            return new NoContentResult();
        }
        catch (BoxNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not Found", "Box was not found for this user"));
        }
    }
}