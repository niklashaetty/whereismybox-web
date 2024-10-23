using System.Text;
using Api;
using Api.Auth;
using Functions.HttpTriggers.Boxes;
using Functions.HttpTriggers.Boxes.Items;
using Functions.HttpTriggers.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace NarrowIntegrationTests;

public class Driver
{
    private readonly Fixture _fixture;
    public Guid? authenticatedUserId { get; set; }

    public Driver(Fixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    public async Task<IActionResult> InvokeCreateBoxFunction(CreateBoxRequest request, string collectionId)
    {
        var sut = new CreateBoxV2Function(_fixture.CreateBoxCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);

        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }
        
        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeGetBoxFunction(string collectionId, Guid boxId)
    {
        var sut = new GetBoxV2Function(_fixture.GetBoxQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeGetUnattachedItems(string collectionId)
    {
        var sut = new GetUnattachedItemsV2Function(_fixture.XunitLoggerFactory,
            _fixture.GetUnattachedItemsQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeDeleteBoxFunction(string collectionId, Guid boxId)
    {
        var sut = new DeleteBoxV2Function(_fixture.DeleteBoxCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeMoveItemFunction(MoveItemRequest request, string collectionId, Guid boxId,
        Guid itemId)
    {
        var sut = new MoveItemFunction(_fixture.MoveItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId, boxId, itemId);
    }

    public async Task<IActionResult> InvokeRemoveItemFunction(string collectionId, Guid boxId, Guid itemId,
        bool isHardDelete)
    {
        var sut = new RemoveItemV2Function(_fixture.DeleteItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        httpRequest.Query = new QueryCollection(new Dictionary<string, StringValues>
            {{"hardDelete", isHardDelete.ToString()}});

        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }
        
        return await sut.RunAsync(httpRequest, collectionId, boxId, itemId);
    }

    public async Task<IActionResult> InvokeDeleteUnattachedItemFunction(string collectionId, Guid itemId)
    {
        var sut = new DeleteUnattachedItemV2Function(_fixture.DeleteUnattachedItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId, itemId);
    }

    public async Task<IActionResult> InvokeMoveUnattachedItemToBoxFunction(MoveUnattachedItemToBoxRequest request,
        string collectionId, Guid itemId)
    {
        var sut = new MoveUnattachedItemToBoxV2Function(_fixture.MoveUnattachedItemToBoxCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }
        
        return await sut.RunAsync(httpRequest, collectionId, itemId);
    }

    public async Task<IActionResult> InvokeAddItemFunction(AddItemRequest request, string collectionId, Guid boxId)
    {
        var sut = new AddItemV2Function(_fixture.AddItemCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeGetAllBoxesInCollectionFunction(string collectionId)
    {
        var sut = new GetAllBoxesInCollectionFunction(_fixture.GetBoxCollectionQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokePatchBoxFunction(UpdateBoxRequest request, string collectionId, Guid boxId)
    {
        var sut = new PatchBoxFunction(_fixture.UpdateBoxCommandCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId, boxId);
    }

    public async Task<IActionResult> InvokeCreateCollectionFunction(CreateCollectionRequest request,
        Guid userId)
    {
        var sut = new CreateCollectionFunction(_fixture.CreateCollectionCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest(request);
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, userId);
    }

    public async Task<IActionResult> InvokeDeleteCollectionFunction(string collectionId)
    {
        var sut = new DeleteCollectionFunction(_fixture.GetUserPermissionsQueryHandler,
            _fixture.DeleteCollectionCommandHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeGetCollectionFunction(string collectionId)
    {
        var sut = new GetCollectionFunction(_fixture.XunitLoggerFactory, _fixture.GetCollectionQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeGetCollectionOwnerFunction(string collectionId)
    {
        var sut = new GetCollectionOwnerFunction(_fixture.XunitLoggerFactory, _fixture.GetCollectionOwnerQueryHandler,
            _fixture.GetUserPermissionsQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, collectionId);
    }

    public async Task<IActionResult> InvokeGetOwnedCollectionsFunction(Guid userId)
    {
        var sut = new GetOwnedCollectionsFunction(_fixture.XunitLoggerFactory, _fixture.GetOwnedCollectionQueryHandler,
            _fixture.GetContributorCollectionsQueryHandler,
            _fixture.GetUserPermissionsQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        
        httpRequest.Query = new QueryCollection(new Dictionary<string, StringValues>
            {{"filter", "owner"}});
        
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, userId);
    }
    
    public async Task<IActionResult> InvokeGetCollectionsIContributeToFunction(Guid userId)
    {
        var sut = new GetOwnedCollectionsFunction(_fixture.XunitLoggerFactory, _fixture.GetOwnedCollectionQueryHandler,
            _fixture.GetContributorCollectionsQueryHandler,
            _fixture.GetUserPermissionsQueryHandler);
        var httpRequest = RequestHelper.CreateHttpRequest();
        httpRequest.Query = new QueryCollection(new Dictionary<string, StringValues>
            {{"filter", "contributor"}});
        if (authenticatedUserId.HasValue)
        {
            httpRequest.AddFullyAuthenticatedUser(externalUserId: Guid.NewGuid().ToString(),
                authenticatedUserId.Value.ToString());
        }

        return await sut.RunAsync(httpRequest, userId);
    }
}

public class DriverBuilder
{
    private readonly Driver _driver;

    public DriverBuilder(Fixture fixture)
    {
        _driver = new Driver(fixture); // Mandatory parameter passed to the Car constructor
    }

    public DriverBuilder WithAuthenticatedUser(Guid userId)
    {
        _driver.authenticatedUserId = userId;
        return this;
    }

    public Driver Build()
    {
        return _driver;
    }
}