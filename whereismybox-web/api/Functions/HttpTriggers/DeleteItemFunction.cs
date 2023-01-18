using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Services.ItemDeletionService;
using Infrastructure.BoxRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace Functions.HttpTriggers;

public class DeleteItemFunction
{
    private readonly IItemDeletionService _itemDeletionService;

    public DeleteItemFunction(IItemDeletionService itemDeletionService)
    {
        ArgumentNullException.ThrowIfNull(itemDeletionService);
        _itemDeletionService = itemDeletionService;
    }

    [OpenApiOperation(operationId: "DeleteItem", tags: new[] {"Items"}, Summary = "Remove item from box")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(AddItemRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName("DeleteItemFunction")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{userId}/boxes/{boxId}/items/{itemId}")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        Guid itemId,
        ILogger log)
    {
        log.LogInformation("Remove a item {ItemId} for user {UserId} and box {BoxId}", itemId, userId, boxId);

        try
        {
            await _itemDeletionService.DeleteItem(userId, boxId, itemId);
            return new NoContentResult();
        }
        catch (BoxNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not Found", "Box was not found for this user"));
        }
    }
}