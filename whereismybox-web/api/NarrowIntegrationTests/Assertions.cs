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
}