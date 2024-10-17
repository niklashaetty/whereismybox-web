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

namespace Functions.HttpTriggers.Contributor;

public class AddContributorFunction
{
    private const string OperationId = "AddContributor";
    private const string FunctionName = OperationId + "Function";
    private readonly ICommandHandler<AddContributorCommand> _commandHandler;
    private readonly IQueryHandler<GetUserPermissionsQuery, Permissions> _permissionsQueryHandler;

    public AddContributorFunction(IQueryHandler<GetUserPermissionsQuery, Permissions> permissionsQueryHandler,
        ICommandHandler<AddContributorCommand> commandHandler)
    {
        ArgumentNullException.ThrowIfNull(permissionsQueryHandler);
        ArgumentNullException.ThrowIfNull(commandHandler);
        _permissionsQueryHandler = permissionsQueryHandler;
        _commandHandler = commandHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Contributors"}, Summary = "Allow another user to edit a collection")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(AddContributorRequest))]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "collections/{collectionId}/contributors")]
        HttpRequest req,
        string collectionId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var addContributorRequest = JsonConvert.DeserializeObject<AddContributorRequest>(body);

            var userId = req.ParseUserId();
            var permissions = await _permissionsQueryHandler.Handle(new GetUserPermissionsQuery(userId));

            var command =
                new AddContributorCommand(permissions, userId, addContributorRequest.Username, domainCollectionId);
            await _commandHandler.Execute(command);

            return new NoContentResult();
        }
        catch (UserNotFoundException)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "Box was not found"));
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