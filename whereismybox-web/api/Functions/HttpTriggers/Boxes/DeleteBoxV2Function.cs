using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.Boxes;

public class DeleteBoxV2Function
{
    private const string OperationId = "DeleteBoxV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<DeleteBoxCommand> _commandHandler;

    public DeleteBoxV2Function(ICommandHandler<DeleteBoxCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Boxes"}, Summary = "Delete a box")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "collections/{collectionId}/boxes/{boxId}")]
        HttpRequest req,
        string collectionId,
        Guid boxId)
    {
        try
        {
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
                return new BadRequestObjectResult(
                    new ErrorResponse("Validation error", "Invalid collectionId"));

            await _commandHandler.Execute(new DeleteBoxCommand(req.ParseUserId(), domainCollectionId,
                new BoxId(boxId)));
            return new NoContentResult();
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