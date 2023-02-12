using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Models;
using Domain.Services.ItemEditingService;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers;

public class EditItemFunction
{
    private const string OperationId = "EditItem";
    private const string FunctionName = OperationId + "Function";
    private readonly IItemEditingService _itemEditingService;

    public EditItemFunction(IItemEditingService itemEditingService)
    {
        ArgumentNullException.ThrowIfNull(itemEditingService);
        _itemEditingService = itemEditingService;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Items"}, Summary = "Edit existing item in box")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
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