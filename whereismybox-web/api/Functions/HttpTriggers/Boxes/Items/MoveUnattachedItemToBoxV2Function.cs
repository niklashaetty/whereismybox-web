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
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class MoveUnattachedItemToBoxV2Function
{
    private const string OperationId = "MoveUnattachedItemToBoxV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<MoveUnattachedItemToBoxCommand> _commandHandler;

    public MoveUnattachedItemToBoxV2Function(ICommandHandler<MoveUnattachedItemToBoxCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"UnattachedItems"},
        Summary = "Move an unattached item to a new box")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(AddItemRequest))]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("itemId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(ItemDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "collections/{collectionId}/unattached-items/{itemId}")]
        HttpRequest req,
        string collectionId,
        Guid itemId)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var moveUnattachedItemToBoxRequest = JsonConvert.DeserializeObject<MoveUnattachedItemToBoxRequest>(body);

        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
        {
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
        }

        try
        {
            var externalUser = req.ParseExternalUser();
            var command = new MoveUnattachedItemToBoxCommand(externalUser.ExternalUserId, domainCollectionId,
                new BoxId(moveUnattachedItemToBoxRequest.BoxId), new ItemId(itemId));
            await _commandHandler.Execute(command);

            return new NoContentResult();
        }
        catch (UnparsableExternalUserException e)
        {
            return new UnauthorizedResult();
        }
        catch (ForbiddenCollectionAccessException)
        {
            return new StatusCodeResult(403);
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