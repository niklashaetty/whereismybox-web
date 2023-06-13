using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Api;
using Api.Auth;
using Domain.CommandHandlers;
using Domain.Commands;
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
using Newtonsoft.Json;

namespace Functions.HttpTriggers.V2;

public class AssignUserRolesFunction
{
    private const string OperationId = "GetRoles";
    private const string FunctionName = OperationId + "Function";
    private readonly IQueryHandler<GetUserByExternalUserIdQuery, User> _getUserByExternalIdQueryHandler;
    private readonly ICommandHandler<CreateUserCommand> _createUserCommandHandler;

    public AssignUserRolesFunction(IQueryHandler<GetUserByExternalUserIdQuery, User> getUserByExternalIdQueryHandler, ICommandHandler<CreateUserCommand> createUserCommandHandler)
    {
        ArgumentNullException.ThrowIfNull(getUserByExternalIdQueryHandler);
        ArgumentNullException.ThrowIfNull(createUserCommandHandler);
        _getUserByExternalIdQueryHandler = getUserByExternalIdQueryHandler;
        _createUserCommandHandler = createUserCommandHandler;
    }

    [OpenApiOperation(operationId: OperationId, tags: new[] {"Users"}, Summary = "Fetch the user object of the current logged in user")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(RolesResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "Invalid request")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, MediaTypeNames.Application.Json, typeof(ErrorResponse),
        Summary = "User was not found")]
    [FunctionName("GetRoles")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetRoles")] 
        HttpRequest req,
        ILogger log)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation("[AssignRolesRequest]: {Body}", body);
            var rolesRequest = JsonConvert.DeserializeObject<RolesRequest>(body);
            var externalUser = rolesRequest.AsExternalUser();

            var user = await _getUserByExternalIdQueryHandler.Handle(new GetUserByExternalUserIdQuery(externalUser.ExternalUserId));
            var response = user.AsRolesResponse();
            var asString = JsonConvert.SerializeObject(response);
            log.LogInformation("AssignRolesResponse]: {AsString}", asString);
            return new OkObjectResult(response);
        }
        catch (UserNotFoundException)
        {
            return new NotFoundObjectResult(new ErrorResponse("Not found", "User not found"));
        }
    }
}