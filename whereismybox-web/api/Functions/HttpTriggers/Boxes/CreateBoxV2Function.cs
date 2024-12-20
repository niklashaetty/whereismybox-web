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
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.Boxes;

public class CreateBoxV2Function
{
    private const string OperationId = "CreateBoxV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<CreateBoxCommand> _commandHandler;

    public CreateBoxV2Function(ICommandHandler<CreateBoxCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Boxes"},
        Summary = "Creates a new box in a given collection")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateBoxRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [Function(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "collections/{collectionId}/boxes")]
        HttpRequest req,
        string collectionId)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var createBoxRequest = JsonConvert.DeserializeObject<CreateBoxRequest>(body);
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
                return new BadRequestObjectResult(
                    new ErrorResponse("Validation error", "Invalid collectionId"));

            var boxId = new BoxId();
            var command = new CreateBoxCommand(req.ParseUserId(), domainCollectionId, boxId,
                createBoxRequest.Number, createBoxRequest.Name);

            await _commandHandler.Execute(command);
            return new CreatedResult($"/api/collections/{command.CollectionId}/boxes/{command.BoxId}",
                new CreateBoxResponse(command.CollectionId.Value, command.BoxId.Value));
        }
        catch (NonUniqueBoxException)
        {
            return new ConflictObjectResult(new ErrorResponse("Conflict",
                "A box with this number already exist in this collection"));
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