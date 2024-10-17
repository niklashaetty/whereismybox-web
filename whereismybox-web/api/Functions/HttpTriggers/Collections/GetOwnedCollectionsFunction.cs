using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Authorization;
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

namespace Functions.HttpTriggers.Collections;

public class GetCollectionsFunction
{
    private const string OperationId = "GetMyCollectionOwnerFunction";
    private const string FunctionName = OperationId + "Function";

    private readonly IQueryHandler<GetContributorCollectionsQuery, List<Collection>>
        _contributedCollectionsQueryHandler;

    private readonly IQueryHandler<GetOwnedCollectionQuery, List<Collection>> _ownedCollectionsQueryHandler;

    private readonly IQueryHandler<GetUserPermissionsQuery, Permissions> _permissionsQueryHandler;

    public GetCollectionsFunction(ILoggerFactory loggerFactory,
        IQueryHandler<GetOwnedCollectionQuery, List<Collection>> ownedCollectionsQueryHandler,
        IQueryHandler<GetContributorCollectionsQuery, List<Collection>> contributedCollectionsQueryHandler,
        IQueryHandler<GetUserPermissionsQuery, Permissions> permissionsQueryHandler)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(ownedCollectionsQueryHandler);
        ArgumentNullException.ThrowIfNull(contributedCollectionsQueryHandler);
        ArgumentNullException.ThrowIfNull(permissionsQueryHandler);
        _ownedCollectionsQueryHandler = ownedCollectionsQueryHandler;
        _contributedCollectionsQueryHandler = contributedCollectionsQueryHandler;
        _permissionsQueryHandler = permissionsQueryHandler;
    }

    [OpenApiOperation(OperationId, "Collections",
        Summary = "Get the collection the user owns or contributes to")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(List<CollectionDto>))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}/collections")]
        HttpRequest req,
        Guid userId)
    {
        try
        {
            var filter = req.Query["filter"].ToString();
            var tokenUserId = req.ParseUserId();
            var pathUserId = new UserId(userId);
            if (tokenUserId != pathUserId) return new ForbidResult();

            var permissions = await _permissionsQueryHandler.Handle(new GetUserPermissionsQuery(req.ParseUserId()));
            if (filter.Equals("owner"))
            {
                var ownedCollections =
                    await _ownedCollectionsQueryHandler.Handle(
                        new GetOwnedCollectionQuery(permissions, pathUserId));
                return new OkObjectResult(ownedCollections.Select(c => c.ToApiModel()));
            }

            if (filter.Equals("contributor"))
            {
                var collectionIContributeTo =
                    await _contributedCollectionsQueryHandler.Handle(
                        new GetContributorCollectionsQuery(permissions, pathUserId));
                return new OkObjectResult(collectionIContributeTo.Select(c => c.ToApiModel()));
            }

            return new BadRequestObjectResult(new ErrorResponse("Validation error",
                "Invalid filter, owner or contributor is allowed"));
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