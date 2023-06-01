using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Exceptions;
using Domain.Services.UnattachedItemFetchingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V1;

public class GetUnattachedItemsFunction
{
    private const string OperationId = "GetUnattachedItems";
    private const string FunctionName = OperationId + "Function";
    private readonly IUnattachedItemFetchingService _unattachedItemFetchingService;
    private readonly ILogger _logger;

    public GetUnattachedItemsFunction(ILoggerFactory loggerFactory, IUnattachedItemFetchingService unattachedItemFetchingService)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(unattachedItemFetchingService);
        _logger = loggerFactory.CreateLogger<GetUnattachedItemsFunction>();
        _unattachedItemFetchingService = unattachedItemFetchingService;
    }

    [OpenApiOperation(operationId:OperationId, tags: new[] {"Items"},
        Summary = "Get items not tied to a box for a given user")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(List<UnattachedItemDto>))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{userId}/items")]
        HttpRequest req,
        Guid userId,
        ILogger log)
    {
        try
        {
            var unattachedItemCollection = await _unattachedItemFetchingService.Get(userId);
            return new OkObjectResult(unattachedItemCollection.UnattachedItems.Select(i  => i.ToApiModel()));
        }
        catch (UnattachedItemsNotFoundException)
        {
            return new OkObjectResult(new List<UnattachedItemDto>());
        }
    }
}