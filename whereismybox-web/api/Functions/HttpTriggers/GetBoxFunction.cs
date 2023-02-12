using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain;
using Domain.Exceptions;
using Domain.Repositories;
using Functions.Mappers;
using Infrastructure.BoxRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers;

public class GetBoxFunction
{
    private const string OperationId = "GetBox";
    private const string FunctionName = OperationId + "Function";
    private readonly IBoxRepository _boxRepository;

    public GetBoxFunction(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Boxes"}, Summary = "Get a box for a given user and all its contents")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{userId}/boxes/{boxId}")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        ILogger log)
    {
        try
        {
            var newBox = await _boxRepository.Get(userId, boxId);
            return new OkObjectResult(newBox.ToApiModel());
        }
        catch (BoxNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "Box was not found"));
        }
    }
}