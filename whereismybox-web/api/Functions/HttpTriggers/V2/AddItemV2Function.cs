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
using Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class AddItemV2Function
{
    private const string OperationId = "AddItemV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<AddItemCommand> _commandHandler;

    public AddItemV2Function(ICommandHandler<AddItemCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Items"}, Summary = "Create new item and add it to box")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(AddItemRequest))]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "collections/{collectionId}/boxes/{boxId}/items")]
        HttpRequest req,
        string collectionId,
        Guid boxId)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var addItemRequest = JsonConvert.DeserializeObject<AddItemRequest>(body);

        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
        {
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
        }

        try
        {
            var externalUser = req.ParseExternalUser();
            var itemId = new ItemId();
            var command = new AddItemCommand(externalUser.ExternalUserId, domainCollectionId, new BoxId(boxId), itemId,
                addItemRequest.Name, addItemRequest.Description);
            await _commandHandler.Execute(command);

            return new CreatedResult($"/api/collections/{command.CollectionId}/boxes/{command.BoxId}/items/{itemId}",
                new ItemDto(itemId.Value, addItemRequest.Name, addItemRequest.Description));
        }
        catch (BoxNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "Box was not found"));
        }
        catch (InvalidOperationException e)
        {
            return new ConflictObjectResult(new ErrorResponse("Conflict", "Item already exists on this box"));
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