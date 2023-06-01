using Api;
using Xunit;
using Xunit.Abstractions;

namespace NarrowIntegrationTests;

public class E2ETests
{
    private readonly Fixture _fixture;
    private readonly Driver _driver;

    public E2ETests(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture(testOutputHelper);
        _driver = new Driver(_fixture);
    }

    [Fact]
    public async void SimpleE2ETest()
    {
        // When creating a user
        var userName = "Some username";
        var createUserResponse = await _driver.InvokeCreateUserFunction(new CreateUserRequest(userName));

        // Then there should be a user
        ResponseAssertions.AssertSuccessStatusCode(createUserResponse);
        var user = createUserResponse.GetContentOfType<UserDto>();
        var collectionId = user.PrimaryCollectionId;

        // And there should be 0 boxes
        var getBoxesResponse = await _driver.InvokeGetBoxCollectionFunction(user.PrimaryCollectionId);
        Assertions.AssertBoxes(getBoxesResponse, 0);

        // When creating a box
        var createBoxRequest = new CreateBoxRequest("Some box name", 1);
        var createBoxResult = await _driver.InvokeCreateBoxFunction(createBoxRequest, collectionId);
        var boxId = createBoxResult.GetContentOfType<CreateBoxResponse>().BoxId;
        
        // Then there should be 1 box
        getBoxesResponse = await _driver.InvokeGetBoxCollectionFunction(collectionId);
        Assertions.AssertBoxes(getBoxesResponse, 1);

        // When adding two items to the box
        await _driver.InvokeAddItemFunction(new AddItemRequest("First item", "Description"),
            collectionId, boxId);
        await _driver.InvokeAddItemFunction(new AddItemRequest("Second item", "Description"),
            collectionId, boxId);
        
        // Then there should be two items in the box
        var getBoxResponse = await _driver.InvokeGetBoxFunction(collectionId, boxId);
        var box = Assertions.AssertItems(getBoxResponse, 2);
        
        // When deleting the box
        var deleteBoxResponse = await _driver.InvokeDeleteBoxFunction(collectionId, boxId);
        ResponseAssertions.AssertSuccessStatusCode(deleteBoxResponse);
        
        // Then the box should be deleted
        getBoxResponse = await _driver.InvokeGetBoxFunction(collectionId, boxId);
        ResponseAssertions.Assert404NotFound(getBoxResponse);
        
        // And there should be no box
        getBoxesResponse = await _driver.InvokeGetBoxCollectionFunction(collectionId);
        Assertions.AssertBoxes(getBoxesResponse, 0);
    }
    
    [Fact]
    public async void UnattachedItemScenarioTests()
    {
        // When creating a user
        var userName = "Some username";
        var createUserResponse = await _driver.InvokeCreateUserFunction(new CreateUserRequest(userName));

        // Then there should be a user
        ResponseAssertions.AssertSuccessStatusCode(createUserResponse);
        var user = createUserResponse.GetContentOfType<UserDto>();
        var collectionId = user.PrimaryCollectionId;

        // And there should be 0 boxes
        var getBoxesResponse = await _driver.InvokeGetBoxCollectionFunction(user.PrimaryCollectionId);
        Assertions.AssertBoxes(getBoxesResponse, 0);

        // When creating a box
        var createBoxRequest = new CreateBoxRequest("Some box name", 1);
        var createBoxResult = await _driver.InvokeCreateBoxFunction(createBoxRequest, collectionId);
        var boxId = createBoxResult.GetContentOfType<CreateBoxResponse>().BoxId;
        
        // Then there should be 1 box
        getBoxesResponse = await _driver.InvokeGetBoxCollectionFunction(collectionId);
        Assertions.AssertBoxes(getBoxesResponse, 1);

        // When adding an item to the box
        var addItemResponse = await _driver.InvokeAddItemFunction(new AddItemRequest("First item", "Description"),
            collectionId, boxId);
        ResponseAssertions.AssertSuccessStatusCode(addItemResponse);
        var itemId = addItemResponse.GetContentOfType<ItemDto>().ItemId;

        // Then there should be one item in the box
        var getBoxResponse = await _driver.InvokeGetBoxFunction(collectionId, boxId);
        Assertions.AssertItems(getBoxResponse, 1);
        
        // And no unattached items TODO
        
        // When hard deleting the item
        var hardDeleteItemResponse = await _driver.InvokeRemoveItemFunction(collectionId, boxId, itemId, true);
        ResponseAssertions.AssertSuccessStatusCode(hardDeleteItemResponse);
        
        // Then there should be no items in the box
        getBoxResponse = await _driver.InvokeGetBoxFunction(collectionId, boxId);
        var box = Assertions.AssertItems(getBoxResponse, 0);
        
        // When adding an item to the box
        addItemResponse = await _driver.InvokeAddItemFunction(new AddItemRequest("First item", "Description"),
            collectionId, boxId);
        ResponseAssertions.AssertSuccessStatusCode(addItemResponse);
        itemId = addItemResponse.GetContentOfType<ItemDto>().ItemId;
        
        // When removing an item from the box
        var removeItemResponse = await _driver.InvokeRemoveItemFunction(collectionId, boxId, itemId, false);
        ResponseAssertions.AssertSuccessStatusCode(removeItemResponse);
        
        // Then there should be no items in the box
        getBoxResponse = await _driver.InvokeGetBoxFunction(collectionId, boxId);
        Assertions.AssertItems(getBoxResponse, 0);
        
        // And one unattached item
        
    }
}