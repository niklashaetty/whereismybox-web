using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V1;

public class ListBoxesFunction
{
    private const string OperationId = "ListBoxes";
    private const string FunctionName = OperationId + "Function";
    private readonly IBoxRepository _boxRepository;

    public ListBoxesFunction(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    [OpenApiOperation(operationId:OperationId, tags: new[] {"Boxes"}, Summary = "List all boxes and their contents for a given user")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(List<BoxDto>))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{userId}/boxes")]
        HttpRequest req,
        Guid userId,
        ILogger log)
    {
        var boxes = await _boxRepository.ListBoxesByUser(userId);
        return new OkObjectResult(boxes.Select(box => box.ToApiModel()));
    }
}