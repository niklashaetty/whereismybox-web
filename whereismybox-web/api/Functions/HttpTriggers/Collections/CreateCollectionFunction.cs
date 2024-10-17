using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.Collections;

public class CreateCollectionFunction
{
    private const string OperationId = "CreateCollection";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<CreateCollectionCommand> _commandHandler;

    public CreateCollectionFunction(ICommandHandler<CreateCollectionCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Collections"},
        Summary = "Creates a new collection")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateCollectionRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/{userId}/collections")]
        HttpRequest req,
        string userId)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var createCollectionRequest = JsonConvert.DeserializeObject<CreateCollectionRequest>(body);

            if (UserId.TryParse(userId, out var pathUserId) is false)
                return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid userId"));
            var tokenUserId = req.ParseUserId();
            if (tokenUserId != pathUserId) return new ForbidResult();

            var collectionId = CollectionId.GenerateNew();
            var command = new CreateCollectionCommand(pathUserId, collectionId, createCollectionRequest.Name);

            await _commandHandler.Execute(command);
            return new CreatedResult($"/api/collections/{command.CollectionId}",
                new CreateCollectionResponse(command.CollectionId.Value, command.Name));
        }
        catch (UnparsableExternalUserException)
        {
            return new UnauthorizedResult();
        }
    }
}