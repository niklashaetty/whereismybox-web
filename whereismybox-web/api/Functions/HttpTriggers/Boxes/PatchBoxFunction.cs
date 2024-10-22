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

namespace Functions.HttpTriggers.Boxes;

public class PatchBoxFunction
{
    private const string OperationId = "PatchBox";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<UpdateBoxCommand> _commandHandler;

    public PatchBoxFunction(ICommandHandler<UpdateBoxCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Boxes"},
        Summary = "Update a box metadata")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(UpdateBoxRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json,
        typeof(ErrorResponse), Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.Conflict, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "The new box number is already taken")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "collections/{collectionId}/boxes/{boxId}")]
        HttpRequest req,
        string collectionId,
        Guid boxId)
    {
        try
        {
            var externalUser = req.ParseExternalUser();
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var patchBoxRequest = JsonConvert.DeserializeObject<UpdateBoxRequest>(body);
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
            {
                return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
            }

            var command = new UpdateBoxCommand(externalUser.ExternalUserId, domainCollectionId, new BoxId(boxId),
                patchBoxRequest.Number, patchBoxRequest.Name);

            await _commandHandler.Execute(command);
            return new OkObjectResult(new UpdateBoxResponse(collectionId, boxId, patchBoxRequest.Name,
                patchBoxRequest.Number));
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