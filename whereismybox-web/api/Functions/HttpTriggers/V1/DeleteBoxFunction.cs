using System;
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

public class DeleteBoxFunction
{
    private const string OperationId = "DeleteBox";
    private const string FunctionName = OperationId + "Function";
    private readonly IBoxRepository _boxRepository;

    public DeleteBoxFunction(IBoxRepository boxRepository)
    {
        ArgumentNullException.ThrowIfNull(boxRepository);
        _boxRepository = boxRepository;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Boxes"}, Summary = "Delete a box")]
    [OpenApiParameter("userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("boxId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{userId}/boxes/{boxId}")]
        HttpRequest req,
        Guid userId,
        Guid boxId,
        ILogger log)
    {
        log.LogInformation("Delete box {BoxId} for user {UserId}" , boxId, userId);
        
        await _boxRepository.Delete(userId, boxId);
        return new NoContentResult();
    }
}