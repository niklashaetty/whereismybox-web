using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Exceptions;
using Domain.Services.ItemAddingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V1;

public class CreateItemFunction
{
    private const string OperationId = "CreateItem";
    private const string FunctionName = OperationId + "Function";
    private readonly IItemAddingService _itemAddingService;

    public CreateItemFunction(IItemAddingService itemAddingService)
    {
        ArgumentNullException.ThrowIfNull(itemAddingService);
        _itemAddingService = itemAddingService;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Items"}, Summary = "Create new item and add it to box")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(AddItemRequest))]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{userId}/boxes/{boxId}/items")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        ILogger log)
    {
        log.LogInformation("Add a new item for user {UserId} and box {BoxId}", userId, boxId);
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var addItemRequest = JsonConvert.DeserializeObject<AddItemRequest>(body);

        try
        {
            var newItem = await _itemAddingService.CreateItem(userId, boxId, addItemRequest.Name, addItemRequest.Description);
            return new CreatedResult($"/api/users/{userId}/boxes/{boxId}/items/{newItem.ItemId}",
                new ItemDto(newItem.ItemId, newItem.Name, newItem.Description));
        }
        catch (BoxNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "Box was not found"));
        }
        catch (InvalidOperationException e)
        {
            return new ConflictObjectResult(new ErrorResponse("Conflict", "Item already exists on this box"));
        }
    }
}