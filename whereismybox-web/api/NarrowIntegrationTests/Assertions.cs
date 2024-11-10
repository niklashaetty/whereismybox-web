using Api;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace NarrowIntegrationTests;

public static class Assertions
{
    
    public static BoxCollectionDto AssertBoxes(this IActionResult actionResult, int amount)
    {
        actionResult.AssertSuccessStatusCode();
        var response = actionResult.GetContentOfType<BoxCollectionDto>();
        Assert.Equal(amount, response.Boxes.Count);
        return response;
    }
    
    public static BoxDto AssertBoxDetails(this IActionResult actionResult, string name, int number, int? items=null)
    {
        actionResult.AssertSuccessStatusCode();
        var response = actionResult.GetContentOfType<BoxDto>();
        Assert.Equal(name, response.Name);
        Assert.Equal(number, response.Number);
        if (items is not null)
        {
            Assert.Equal(response.Items.Count, items);
        }
        return response;
    }
    
    public static CreateBoxResponse AssertBoxCreated(this IActionResult actionResult)
    {
        actionResult.AssertSuccessStatusCode();
        var response = actionResult.GetContentOfType<CreateBoxResponse>();
        return response;
    }
    
    public static BoxDto AssertItems(this IActionResult actionResult, int amount)
    {
        actionResult.AssertSuccessStatusCode();
        var response = actionResult.GetContentOfType<BoxDto>();
        Assert.Equal(amount, response.Items.Count);
        return response;
    }
    
    public static List<CollectionDto> AssertCollections(this IActionResult actionResult, int amount)
    {
        actionResult.AssertSuccessStatusCode();
        
        var okResult = actionResult as OkObjectResult;
        var response = okResult.Value as List<CollectionDto>;
        Assert.Equal(amount, response.Count);
        return response;
    }
    
    public static UnattachedItemCollectionDto AssertUnattachedItems(this IActionResult actionResult, int amount)
    {
        actionResult.AssertSuccessStatusCode();
        var response = actionResult.GetContentOfType<UnattachedItemCollectionDto>();
        Assert.Equal(amount, response.UnattachedItems.Count);
        return response;
    }
    
    public static void AssertItemInBox(this IActionResult actionResult, Guid itemId)
    {
        actionResult.AssertSuccessStatusCode();
        var response = actionResult.GetContentOfType<BoxDto>();
        var item = response.Items.FirstOrDefault(i => i.ItemId == itemId);
        Assert.NotNull(item);
    }
}