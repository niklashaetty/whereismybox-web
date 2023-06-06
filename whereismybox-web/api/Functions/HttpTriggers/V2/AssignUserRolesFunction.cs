using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
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

namespace Functions.HttpTriggers.V2;

public class AssignUserRolesFunction
{
    private const string OperationId = "GetRoles";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _queryHandler;

    public AssignUserRolesFunction(IQueryHandler<GetUserByExternalUserIdQuery, User> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Fetch the user object of the current logged in user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not found")]
    [FunctionName("GetRoles")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetRoles")] 
        HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Entering AssignUserRolesFunction");
        log.LogWarning("Entering AssignUserRolesFunction");
        return new OkObjectResult(new RolesResponse()
        {
            Roles = new List<string>(){"MyCoolRole", "secondRole"}
        });
        using (var content = new StreamContent(req.Body))
        {
            var contentString = await content.ReadAsStringAsync();
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