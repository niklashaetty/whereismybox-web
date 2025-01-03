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
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.Boxes.Items;

public class GetUnattachedItemsV2Function
{
    private const string OperationId = "GetUnattachedItemsV2";
    private const string FunctionName = OperationId + "Function";
    private readonly ILogger _logger;
    private readonly IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>> _queryHandler;

    public GetUnattachedItemsV2Function(ILoggerFactory loggerFactory,
        IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(queryHandler);
        _logger = loggerFactory.CreateLogger<GetUnattachedItemsV2Function>();
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(OperationId, new[] {"UnattachedItems"},
        Summary = "Get unattached items in a collection")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UnattachedItemCollectionDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [Function(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "collections/{collectionId}/unattached-items")]
        HttpRequest req,
        string collectionId)
    {
        try
        {
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
                return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));

            var unattachedItems =
                await _queryHandler.Handle(new GetUnattachedItemsQuery(req.ParseUserId(),
                    domainCollectionId));
            return new OkObjectResult(new UnattachedItemCollectionDto(domainCollectionId.Value,
                unattachedItems.Select(b => b.ToApiModel()).ToList()));
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