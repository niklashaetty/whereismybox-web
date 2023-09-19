using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class CreateCollectionResponse
{
    [JsonRequired]
    [OpenApiProperty(Description = "The unique identifier for a collection", Default = "2ss21ab")]
    public string CollectionId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The collection name", Default = "Basement")]
    public string Name { get; set; }

    public CreateCollectionResponse(string collectionId, string name)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(name);
        CollectionId = collectionId;
        Name = name;
    }
}