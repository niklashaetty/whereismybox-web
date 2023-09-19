using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.QueryHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class CreateCollectionFunction
{
    private const string OperationId = "CreateCollection";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<CreateCollectionCommand> _commandHandler;
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _queryHandler;

    public CreateCollectionFunction(ICommandHandler<CreateCollectionCommand> commandHandler,
        IQueryHandler<GetUserByExternalUserIdQuery, User> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(commandHandler);
        ArgumentNullException.ThrowIfNull(queryHandler);
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Collections"},
        Summary = "Creates a new collection")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateCollectionRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "collections")]
        HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var createCollectionRequest = JsonConvert.DeserializeObject<CreateCollectionRequest>(body);
            
            var externalUser = req.ParseExternalUser();
            var user = await _queryHandler.Handle(new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));

            var collectionId = CollectionId.GenerateNew();
            var command = new CreateCollectionCommand(user.UserId, collectionId, createCollectionRequest.Name);

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