using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Primitives;
using Domain.Queries;
using Domain.QueryHandlers;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V2;

public class GetCollectionContributorsFunction
{
    private const string OperationId = "GetCollectionContributors";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetCollectionContributorsQuery, List<User>> _queryHandler;
    private readonly IQueryHandler<GetUserPermissionsQuery, Permissions> _permissionsQueryHandler;
    private readonly ILogger _logger;

    public GetCollectionContributorsFunction(ILoggerFactory loggerFactory,
        IQueryHandler<GetCollectionContributorsQuery, List<User>> queryHandler,
        IQueryHandler<GetUserPermissionsQuery, Permissions> permissionsQueryHandler)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(queryHandler);
        ArgumentNullException.ThrowIfNull(permissionsQueryHandler);
        _logger = loggerFactory.CreateLogger<GetCollectionContributorsFunction>();
        _queryHandler = queryHandler;
        _permissionsQueryHandler = permissionsQueryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"UnattachedItems"},
        Summary = "Get unattached items in a collection")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(List<CollectionContributor>))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "collections/{collectionId}/contributors")]
        HttpRequest req,
        string collectionId)
    {
        try
        {
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
            {
                return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
            }
            var permissions = await _permissionsQueryHandler.Handle(new GetUserPermissionsQuery(req.ParseUserId()));
            var users = await _queryHandler.Handle(new GetCollectionContributorsQuery(permissions, domainCollectionId));
            return new OkObjectResult(users.Select(u => u.ToApiCollectionContributor()));
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