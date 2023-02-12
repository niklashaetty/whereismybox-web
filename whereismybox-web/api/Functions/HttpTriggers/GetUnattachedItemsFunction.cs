using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Exceptions;
using Domain.Repositories;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers;

public class GetUnattachedItemsFunction
{
    private const string OperationId = "GetUnattachedItems";
    private const string FunctionName = OperationId + "Function";
    private readonly IUnattachedItemRepository _unattachedItemRepository;

    public GetUnattachedItemsFunction(IUnattachedItemRepository unattachedItemRepository)
    {
        ArgumentNullException.ThrowIfNull(unattachedItemRepository);
        _unattachedItemRepository = unattachedItemRepository;
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
            var unattachedItemCollection = await _unattachedItemRepository.Get(userId);
            return new OkObjectResult(unattachedItemCollection.UnattachedItems.Select(i  => i.ToApiModel()));
        }
        catch (UnattachedItemsNotFoundException)
        {
            return new OkObjectResult(new List<UnattachedItemDto>());
        }
    }
}