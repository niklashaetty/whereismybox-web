using Api;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace NarrowIntegrationTests;

public static class Assertions
{
    
    public static BoxCollectionDto AssertBoxes(IActionResult actionResult, int amount)
    {
        ResponseAssertions.AssertSuccessStatusCode(actionResult);
        var response = actionResult.GetContentOfType<BoxCollectionDto>();
        Assert.Equal(amount, response.Boxes.Count);
        return response;
    }
    
    public static BoxDto AssertItems(IActionResult actionResult, int amount)
    {
        ResponseAssertions.AssertSuccessStatusCode(actionResult);
        var response = actionResult.GetContentOfType<BoxDto>();
        Assert.Equal(amount, response.Items.Count);
        return response;
    }
    
    public static List<CollectionDto> AssertCollections(IActionResult actionResult, int amount)
    {
        ResponseAssertions.AssertSuccessStatusCode(actionResult);
        
        var okResult = actionResult as OkObjectResult;
        var response = okResult.Value as List<CollectionDto>;
        Assert.Equal(amount, response.Count);
        return response;
    }
    
    public static UnattachedItemCollectionDto AssertUnattachedItems(IActionResult actionResult, int amount)
    {
        ResponseAssertions.AssertSuccessStatusCode(actionResult);
        var response = actionResult.GetContentOfType<UnattachedItemCollectionDto>();
        Assert.Equal(amount, response.UnattachedItems.Count);
        return response;
    }
    
    public static void AssertItemInBox(IActionResult actionResult, Guid itemId)
    {
        ResponseAssertions.AssertSuccessStatusCode(actionResult);
        var response = actionResult.GetContentOfType<BoxDto>();
        var item = response.Items.FirstOrDefault(i => i.ItemId == itemId);
        Assert.NotNull(item);
    }
}