using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Repositories;
using Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace Functions.HttpTriggers;

public class ListBoxesFunction
{
    private readonly IBoxRepository _boxRepository;

    public ListBoxesFunction(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    [OpenApiOperation(operationId: "ListBoxes", tags: new[] {"Boxes"}, Summary = "List all boxes and their contents for a given user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateBoxRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName("ListBoxesFunction")]
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