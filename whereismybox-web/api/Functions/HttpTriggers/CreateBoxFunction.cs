using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Domain.Services.BoxCreationService;
using Functions.Mappers;
using Infrastructure.UserRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.HttpTriggers;

public class CreateBoxFunction
{
    private readonly IBoxCreationService _boxCreationService;

    public CreateBoxFunction(IBoxCreationService boxCreationService)
    {
        ArgumentNullException.ThrowIfNull(boxCreationService);
        _boxCreationService = boxCreationService;
    }

    [OpenApiOperation(operationId: "CreateBox", tags: new[] {"Boxes"}, Summary = "Creates a new box for a given user")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(CreateBoxRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, MediaTypeNames.Application.Json, typeof(BoxDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [FunctionName("CreateBoxFunction")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{userId}/boxes")]
        HttpRequest req,
        Guid userId,
        ILogger log)
    {
        log.LogInformation("Creating a new box for user {UserId}", userId);
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var createBoxRequest = JsonConvert.DeserializeObject<CreateBoxRequest>(body);

        try
        {
            var newBox = await _boxCreationService.Create(userId, createBoxRequest.Name, createBoxRequest.Number);
            return new CreatedResult($"/api/users/{userId}/boxes/{newBox.BoxId}", newBox.ToApiModel());
        }
        catch (UserNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "User was not found"));
        }
        catch (NonUniqueBoxException e)
        {
            return new ConflictObjectResult(new ErrorResponse("Conflict", "User already have a box with this number"));
        }
    }
}