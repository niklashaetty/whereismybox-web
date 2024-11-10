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

namespace Functions.HttpTriggers.Boxes.Items;

public class MoveItemFunction
{
    private const string OperationId = "MoveItem";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<MoveItemCommand> _moveItemCommandHandler;

    public MoveItemFunction(ICommandHandler<MoveItemCommand> moveItemCommandHandler)
    {
        ArgumentNullException.ThrowIfNull(moveItemCommandHandler);
        _moveItemCommandHandler = moveItemCommandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Items"}, Summary = "Move item to another box")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(MoveItemRequest))]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "collections/{collectionId}/boxes/{boxId}/items/{itemId}/move")]
        HttpRequest req,
        string collectionId,
        Guid boxId,
        Guid itemId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
        {
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
        }
        
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var moveItemRequest = JsonConvert.DeserializeObject<MoveItemRequest>(body);
        
        try
        {
            var command = new MoveItemCommand(req.ParseUserId(), domainCollectionId, new ItemId(itemId),
                new BoxId(boxId), moveItemRequest.TargetBoxNumber);
            await _moveItemCommandHandler.Execute(command);
            return new NoContentResult();
        }
        catch (BoxNotFoundException e)
        {
            return new NotFoundResult();
        }
        catch (BoxWithNumberNotFoundException e)
        {
            return new BadRequestObjectResult("No box with this number was found");
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