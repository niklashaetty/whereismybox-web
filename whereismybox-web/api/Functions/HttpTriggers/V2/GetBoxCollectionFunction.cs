using System;
using System.Collections.Generic;
using System.Linq;
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
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V2;

public class GetBoxCollectionFunction
{
    private const string OperationId = "GetBoxCollection";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetBoxCollectionQuery, List<Box>> _queryHandler;
    private readonly ILogger _logger;

    public GetBoxCollectionFunction(ILoggerFactory loggerFactory,
        IQueryHandler<GetBoxCollectionQuery, List<Box>> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(queryHandler);
        _logger = loggerFactory.CreateLogger<GetBoxCollectionFunction>();
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Collections"},
        Summary = "Create a new collection for a specific user")]
    [OpenApiParameter("collectionId", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(BoxCollectionDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "collections/{collectionId")]
        HttpRequest req,
        string collectionId)
    {
        if (CollectionId.TryParse(collectionId, out var domainCollectionId) is false)
        {
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid collectionId"));
        }

        var boxCollection = await _queryHandler.Handle(new GetBoxCollectionQuery(domainCollectionId));
        return new OkObjectResult(new BoxCollectionDto(domainCollectionId.Value,
            boxCollection.Select(b => b.ToApiModel()).ToList()));
    }
}