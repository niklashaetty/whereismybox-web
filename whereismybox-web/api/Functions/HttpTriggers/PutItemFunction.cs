using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Models;
using Domain.Services.ItemDeletionService;
using Domain.Services.ItemEditingService;
using Functions.Mappers;
using Infrastructure.BoxRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.HttpTriggers;

public class PutItemFunction
{
    private readonly IItemEditingService _itemEditingService;

    public PutItemFunction(IItemEditingService itemEditingService)
    {
        ArgumentNullException.ThrowIfNull(itemEditingService);
        _itemEditingService = itemEditingService;
    }

    [OpenApiOperation(operationId: "EditItem", tags: new[] {"Items"}, Summary = "Edit existing item in box")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName("PutItemFunction")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "users/{userId}/boxes/{boxId}/items/{itemId}")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        Guid itemId,
        ILogger log)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var editItemRequest = JsonConvert.DeserializeObject<ItemDto>(body);
        var item = await _itemEditingService.EditItem(userId, boxId,
            new Item(editItemRequest.ItemId, editItemRequest.Name, editItemRequest.Description));
        return new OkObjectResult(item.ToApiModel());
    }
}