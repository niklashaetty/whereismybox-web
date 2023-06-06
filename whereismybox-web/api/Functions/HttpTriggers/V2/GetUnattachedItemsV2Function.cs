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

public class GetUnattachedItemsV2Function
{
    private const string OperationId = "GetUnattachedItemsV2";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>> _queryHandler;
    private readonly ILogger _logger;

    public GetUnattachedItemsV2Function(ILoggerFactory loggerFactory,
        IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(queryHandler);
        _logger = loggerFactory.CreateLogger<GetUnattachedItemsV2Function>();
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"UnattachedItems"},
        Summary = "Get unattached items in a collection")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UnattachedItemCollectionDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "collections/{collectionId}/unattached-items")]
        HttpRequest req,
        string collectionId)
    {
        try
        {
            var externalUser = req.ParseExternalUser();
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
            {
                return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
            }

            var unattachedItems = await _queryHandler.Handle(new GetUnattachedItemsQuery(externalUser.ExternalUserId, domainCollectionId));
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