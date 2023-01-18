using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Repositories;
using Functions.Mappers;
using Infrastructure.BoxRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace Functions.HttpTriggers;

public class GetBoxFunction
{
    private readonly IBoxRepository _boxRepository;

    public GetBoxFunction(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    [OpenApiOperation(operationId: "GetBox", tags: new[] {"Boxes"}, Summary = "Get a box for a given user and all its contents")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName("GetBoxFunction")]
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