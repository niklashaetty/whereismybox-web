using System;
using System.Net;
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
using Microsoft.OpenApi.Models;

namespace Functions.HttpTriggers.V2;

public class GetUserByCollectionIdV2Function
{
    private const string OperationId = "GetUserByCollectionIdV2";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUserByCollectionIdQuery, User> _queryHandler;

    public GetUserByCollectionIdV2Function(IQueryHandler<GetUserByCollectionIdQuery, User> queryHandler)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        _queryHandler = queryHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Get an existing user by its primary collectionId")]
    [OpenApiParameter("primaryCollectionId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(UserDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not found")]
    [FunctionName(FunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] 
        HttpRequest req,
        ILogger log)
    {
        var bearer = req.Headers["Authorization"];
        var msclaimsPrincipal = req.Headers["X-MS-CLIENT-PRINCIPAL"];
        var msclaimsPrincipal2 = req.Headers["x-ms-client-principal"];
        log.LogInformation("[AuthHeader] : {AuthHeader}", bearer);
        log.LogInformation("[XMSCLIENTPRINCIPAL1] : {AuthHeader}", msclaimsPrincipal);
        log.LogInformation("[XMSCLIENTPRINCIPAL2] : {AuthHeader}", msclaimsPrincipal2);
        
        if (CollectionId.TryParse(req.Query["primaryCollectionId"], out var primaryCollectionId) is false)
        {
            return new BadRequestObjectResult(new ErrorResponse("Validation error", "Invalid primaryCollectionId"));
        }

        try
        {
            var user = await _queryHandler.Handle(new GetUserByCollectionIdQuery(primaryCollectionId));
            return new OkObjectResult(user.ToApiModel());
        }
        catch (UserNotFoundException e)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "User was not found"));
        }
    }
}