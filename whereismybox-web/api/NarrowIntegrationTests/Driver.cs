using System.Text;
using Api;
using Domain.Primitives;
using Functions.HttpTriggers.V2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace NarrowIntegrationTests;

public class Driver
{
    private readonly Fixture _fixture;

    public Driver(Fixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }
    
    public async Task<IActionResult> InvokeCreateUserFunction(CreateUserRequest request)
    {
        var sut = new CreateUserV2Function(_fixture.CreateUserCommandHandler);
        var httpRequest = CreateHttpRequest(request);

        return await sut.RunAsync(httpRequest);
    }
    
    public async Task<IActionResult> InvokeCreateBoxFunction(CreateBoxRequest request, string collectionId)
    {
        var sut = new CreateBoxV2Function(_fixture.CreateBoxCommandHandler);
        var httpRequest = CreateHttpRequest(request);

        return await sut.RunAsync(httpRequest, collectionId);
    }
    
    public async Task<IActionResult> InvokeGetBoxFunction(string collectionId, Guid boxId)
    {
        var sut = new GetBoxV2Function(_fixture.GetBoxQueryHandler);
        var httpRequest = CreateHttpRequest(null);

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }
    
    public async Task<IActionResult> InvokeDeleteBoxFunction(string collectionId, Guid boxId)
    {
        var sut = new DeleteBoxV2Function(_fixture.DeleteBoxCommandHandler);
        var httpRequest = CreateHttpRequest(null);

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }
    
    public async Task<IActionResult> InvokeRemoveItemFunction(string collectionId, Guid boxId, Guid itemId, bool isHardDelete)
    {
        var sut = new RemoveItemV2Function(_fixture.DeleteItemCommandHandler);
        var httpRequest = CreateHttpRequest(null);
        httpRequest.QueryString.Add("hardDelete", isHardDelete.ToString());
        httpRequest.Query = new QueryCollection(new Dictionary<string, StringValues>
        {{"hardDelete", isHardDelete.ToString()}});

        return await sut.RunAsync(httpRequest, collectionId, boxId, itemId);
    }
    
    public async Task<IActionResult> InvokeAddItemFunction(AddItemRequest request, string collectionId, Guid boxId)
    {
        var sut = new AddItemV2Function(_fixture.AddItemCommandHandler);
        var httpRequest = CreateHttpRequest(request);

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }
    
    public async Task<IActionResult> InvokeGetBoxCollectionFunction(string collectionId)
    {
        var sut = new GetBoxCollectionFunction(_fixture.XunitLoggerFactory, _fixture.GetBoxCollectionQueryHandler);
        var httpRequest = CreateHttpRequest(null);

        return await sut.RunAsync(httpRequest, collectionId);
    }
    
    public static HttpRequest CreateHttpRequest(object? body)
    {
        
        var context = new DefaultHttpContext();
        HttpRequest request = context.Request;
        var jsonBody = JsonConvert.SerializeObject(body);
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
        return request;
    }
}

