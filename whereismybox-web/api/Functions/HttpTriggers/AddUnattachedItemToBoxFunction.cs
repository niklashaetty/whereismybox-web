using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain;
using Domain.Exceptions;
using Domain.Services.ItemAddingService;
using Infrastructure.BoxRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers;

public class AddUnattachedItemToBoxFunction
{
    private const string OperationId = "AddUnattachedItem";
    private const string FunctionName = OperationId + "Function";
    private readonly IItemAddingService _itemAddingService;
    private readonly ILogger _logger;

    public AddUnattachedItemToBoxFunction(ILoggerFactory loggerFactory, IItemAddingService itemAddingService)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(itemAddingService);
        _logger = loggerFactory.CreateLogger<AddUnattachedItemToBoxFunction>();
        _itemAddingService = itemAddingService;
    }

    [OpenApiOperation(operationId:OperationId, tags: new[] {"Items"}, Summary = "Add unattached item to box")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{userId}/boxes/{boxId}/items/{itemId}")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        Guid itemId,
        ILogger log)
    {
        log.LogInformation("Add an existing item {ItemId} for user {UserId} and box {BoxId}", itemId, userId, boxId);

        try
        {
            var newItem = await _itemAddingService.AddUnattachedItemToBox(userId, boxId, itemId);
            return new CreatedResult($"/api/users/{userId}/boxes/{boxId}/items/{newItem.ItemId}",
                new ItemDto(newItem.ItemId, newItem.Name, newItem.Description));
        }
        catch (BoxNotFoundException)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "Box was not found"));
        }
        catch (InvalidOperationException)
        {
            return new ConflictObjectResult(new ErrorResponse("Conflict", "Item already exists on this box"));
        }
        catch (UnattachedItemNotFoundException)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "Item was not found"));
        }
    }
}