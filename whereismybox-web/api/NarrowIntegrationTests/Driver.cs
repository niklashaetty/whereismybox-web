using System.Text;
using Api;
using Api.Auth;
using Functions.HttpTriggers.Boxes;
using Functions.HttpTriggers.Boxes.Items;
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

    public async Task<IActionResult> InvokeCreateBoxFunction(CreateBoxRequest request, string collectionId)
    {
        var sut = new CreateBoxV2Function(_fixture.CreateBoxCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeGetBoxFunction(string collectionId, Guid boxId)
    {
        var sut = new GetBoxV2Function(_fixture.GetBoxQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeGetUnattachedItems(string collectionId)
    {
        var sut = new GetUnattachedItemsV2Function(_fixture.XunitLoggerFactory,
            _fixture.GetUnattachedItemsQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeDeleteBoxFunction(string collectionId, Guid boxId)
    {
        var sut = new DeleteBoxV2Function(_fixture.DeleteBoxCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeRemoveItemFunction(string collectionId, Guid boxId, Guid itemId,
        bool isHardDelete)
    {
        var sut = new RemoveItemV2Function(_fixture.DeleteItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        httpRequest.Query = new QueryCollection(new Dictionary<string, StringValues>
            {{"hardDelete", isHardDelete.ToString()}});

        return await sut.RunAsync(httpRequest, collectionId, boxId, itemId);
    }

    public async Task<IActionResult> InvokeDeleteUnattachedItemFunction(string collectionId, Guid itemId)
    {
        var sut = new DeleteUnattachedItemV2Function(_fixture.DeleteUnattachedItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();

        return await sut.RunAsync(httpRequest, collectionId, itemId);
    }

    public async Task<IActionResult> InvokeMoveUnattachedItemToBoxFunction(MoveUnattachedItemToBoxRequest request,
        string collectionId, Guid itemId)
    {
        var sut = new MoveUnattachedItemToBoxV2Function(_fixture.MoveUnattachedItemToBoxCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);

        return await sut.RunAsync(httpRequest, collectionId, itemId);
    }

    public async Task<IActionResult> InvokeAddItemFunction(AddItemRequest request, string collectionId, Guid boxId)
    {
        var sut = new AddItemV2Function(_fixture.AddItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeGetBoxCollectionFunction(string collectionId)
    {
        var sut = new GetAllBoxesInCollectionFunction(_fixture.GetBoxCollectionQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();

        return await sut.RunAsync(httpRequest, collectionId);
    }
}