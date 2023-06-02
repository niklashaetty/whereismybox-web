using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Azure.Core;
using Domain.Exceptions;
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

public class AssignUserRolesFunction
{
    private const string OperationId = "AssignUserRoles";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _queryHandler;
    private readonly ILogger _logger;

    public AssignUserRolesFunction(ILoggerFactory loggerFactory, IQueryHandler<GetUserByExternalUserIdQuery, User> queryHandler)
    {
        _logger = loggerFactory.CreateLogger<AssignUserRolesFunction>();
        ArgumentNullException.ThrowIfNull(queryHandler);
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Fetch the user object of the current logged in user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not found")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetRoles")] 
        HttpRequest req)
    {
        _logger.LogInformation("Entering AssignUserRolesFunction");
        _logger.LogWarning("Entering AssignUserRolesFunction");
        using (var content = new StreamContent(req.Body))
        {
            var contentString = await content.ReadAsStringAsync();
            _logger.LogWarning("Body: " + contentString);
        }
        try
        {
            var user = await _queryHandler.Handle(new GetUserByExternalUserIdQuery(new ExternalUserId("heello")));
            return new OkObjectResult(user.ToApiModel());
        }
        catch (UserNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "User was not found"));
        }
    }

}