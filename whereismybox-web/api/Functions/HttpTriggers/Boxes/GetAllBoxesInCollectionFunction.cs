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
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.Boxes;

public class GetAllBoxesInCollectionFunction
{
    private const string OperationId = "GetBoxCollection";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetBoxCollectionQuery, List<Box>> _queryHandler;

    public GetAllBoxesInCollectionFunction(
        IQueryHandler<GetBoxCollectionQuery, List<Box>> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(OperationId, new[] {"Collections"},
        Summary = "Get an entire collection of boxes and their items")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(BoxCollectionDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [Function(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "collections/{collectionId}/boxes")]
        HttpRequest req,
        string collectionId)
    {
        try
        {
            if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
                return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));

            var boxCollection =
                await _queryHandler.Handle(new GetBoxCollectionQuery(domainCollectionId, req.ParseUserId()));
            return new OkObjectResult(new BoxCollectionDto(domainCollectionId.Value,
                boxCollection.Select(b => b.ToApiModel()).ToList()));
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