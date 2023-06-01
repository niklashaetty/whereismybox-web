using Domain.Primitives;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

public class CollectionIdTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CollectionIdTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestCollectionId()
    {
        for (int i = 0; i < 100; i++)
        {
            _testOutputHelper.WriteLine(CollectionId.GenerateNew().Value.ToString());
        }
    }
}