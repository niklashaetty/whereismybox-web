using Api;
using Xunit;
using Xunit.Abstractions;

namespace NarrowIntegrationTests;

public class E2ETests
{
    private readonly Fixture _fixture;
    private readonly TestDriver _testDriver;

    public E2ETests(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture(testOutputHelper);
        _testDriver = new TestDriver(_fixture);
    }

    [Fact(DisplayName = "Should be able to create and delete a collection")]
    public async void CollectionTests()
    {
        // Given a registered user
        var user = await _testDriver.GivenARegisteredUser();

        var driver = new DriverBuilder(_fixture)
            .WithAuthenticatedUser(user.UserId)
            .Build();

        // When fetching my collections
        var response = await driver.InvokeGetOwnedCollectionsFunction(user.UserId);

        // There should be no collections
        response.AssertCollections(0);

        // When creating a collection
        response = await driver.InvokeCreateCollectionFunction(new CreateCollectionRequest("name"), user.UserId);
        var collection = response.GetContentOfType<CreateCollectionResponse>();

        // There should be one collection
        response = await driver.InvokeGetOwnedCollectionsFunction(user.UserId);
        response.AssertCollections(1);

        // And no boxes in it
        response = await driver.InvokeGetAllBoxesInCollectionFunction(collection.CollectionId);
        response.AssertBoxes(0);

        // When deleting the collection
        await driver.InvokeDeleteCollectionFunction(collection.CollectionId);

        // The collection should no longer exist
        response = await driver.InvokeGetOwnedCollectionsFunction(user.UserId);
        response.AssertCollections(0);
    }

    [Fact(DisplayName = "Should be able to create, update and delete a box")]
    public async void BoxTest()
    {
        // Given a collection
        var user = await _testDriver.GivenARegisteredUser();
        var collection = await _testDriver.GivenACollection(user);
        var driver = new DriverBuilder(_fixture)
            .WithAuthenticatedUser(user.UserId)
            .Build();

        // When creating a box
        const string boxName = "Box name";
        const int boxNumber = 1;
        var response = await driver.InvokeCreateBoxFunction(new CreateBoxRequest(boxName, boxNumber), collection.CollectionId);
        var createdBox = response.AssertBoxCreated();
        
        
        // Then there should be one box
        response = await driver.InvokeGetAllBoxesInCollectionFunction(collection.CollectionId);
        response.AssertBoxes(1);
        
        // And it should not have any items
        response = await driver.InvokeGetBoxFunction(createdBox.CollectionId, createdBox.BoxId);
        var box = response.AssertBoxDetails(boxName, boxNumber, 0);

        // When updating the box name and number
        const string newName = "newName";
        const int newNumber = 2;
        response = await driver.InvokePatchBoxFunction(new UpdateBoxRequest(newName, newNumber),
            collection.CollectionId, box.BoxId);
        response.AssertSuccessStatusCode();
        
        // Then the name and number should be updated
        response = await driver.InvokeGetBoxFunction(collection.CollectionId, box.BoxId);
        response.AssertBoxDetails(newName, newNumber);

        // When deleting the box
        response = await driver.InvokeDeleteBoxFunction(collection.CollectionId, box.BoxId);
        response.AssertSuccessStatusCode();

        // The box should not exist
        response = await driver.InvokeGetBoxFunction(collection.CollectionId, box.BoxId);
        response.Assert404NotFound();
    }

    [Fact(DisplayName = "Should be able to create, update, and delete an item")]
    public async void ItemTest()
    {
        // Given a collection and a box
        var user = await _testDriver.GivenARegisteredUser();


        // When adding an item

        // Then there should be an item in the box

        // When updating item name and description

        // Then the item should have a new name and description

        // When deleting an item then the item should not exist
    }

    [Fact(DisplayName = "Should be able to move item to another box")]
    public async void MovingItemTest()
    {
        // Given a collection with a box with an item

        // When creating a second box

        // And moving the item to the second box

        // The item should be in the second box
    }

    [Fact(DisplayName = "Should be add and delete unattached item")]
    public async void UnattachedItemScenarioTests()
    {
        // Given a collection, a box, and one item

        // When removing an item from a box

        // And fetching unattached items

        // There should be one unattached item

        // When removing the unattached item

        // Then the unattached item should no longer exists


        /*

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

        // And no unattached items
        var getUnattachedItemsResponse = await _driver.InvokeGetUnattachedItems(collectionId);
        Assertions.AssertUnattachedItems(getUnattachedItemsResponse, 0);

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
        getUnattachedItemsResponse = await _driver.InvokeGetUnattachedItems(collectionId);
        Assertions.AssertUnattachedItems(getUnattachedItemsResponse, 1);

        // When deleting the unattached item
        var deleteUnattachedItemResponse = await _driver.InvokeDeleteUnattachedItemFunction(collectionId, itemId);
        ResponseAssertions.AssertSuccessStatusCode(deleteUnattachedItemResponse);

        // Then there should be no unattached items
        getUnattachedItemsResponse = await _driver.InvokeGetUnattachedItems(collectionId);
        Assertions.AssertUnattachedItems(getUnattachedItemsResponse, 0);*/
    }

    [Fact(DisplayName = "Should be able to move unattached item back to a box")]
    public async void MovingUnattachedItemTest()
    {
        // Given a collection, a box, and one item.

        // When removing the item from the box

        // And fetching unattached items

        // There should be one unattached item

        // When moving the unattached item back to its previous box

        // Then the item should be in the new box

        // And there should be no unattached items

        /*
        // When creating a user
        var userName = "Some username";
        var createUserResponse = await _driver.InvokeCreateUserFunction(new CreateUserRequest(userName));

        // Then there should be a user
        ResponseAssertions.AssertSuccessStatusCode(createUserResponse);
        var user = createUserResponse.GetContentOfType<UserDto>();
        var collectionId = user.PrimaryCollectionId;

        // When creating a box
        var createBoxRequest = new CreateBoxRequest("Some box name", 1);
        var createBoxResult = await _driver.InvokeCreateBoxFunction(createBoxRequest, collectionId);
        var boxId = createBoxResult.GetContentOfType<CreateBoxResponse>().BoxId;

        // And adding an item to the box
        var addItemResponse = await _driver.InvokeAddItemFunction(new AddItemRequest("First item", "Description"),
            collectionId, boxId);
        ResponseAssertions.AssertSuccessStatusCode(addItemResponse);
        var itemId = addItemResponse.GetContentOfType<ItemDto>().ItemId;

        // And removing the item from the box
        var removeItemResponse = await _driver.InvokeRemoveItemFunction(collectionId, boxId, itemId, false);
        ResponseAssertions.AssertSuccessStatusCode(removeItemResponse);

        // And moving it back to the previous box
        var moveItemResponse =
            await _driver.InvokeMoveUnattachedItemToBoxFunction(new MoveUnattachedItemToBoxRequest(boxId), collectionId,
                itemId);
        ResponseAssertions.AssertSuccessStatusCode(moveItemResponse);

        // Then the item should be in the previous box
        var getBoxResponse = await _driver.InvokeGetBoxFunction(collectionId, boxId);
        Assertions.AssertItemInBox(getBoxResponse, itemId);
        */
    }
}