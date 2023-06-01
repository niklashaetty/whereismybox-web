using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class BoxCollectionDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The unique identifier for a collection", Default = "2ss")]
    public string CollectionId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The boxes in a collection")]
    public List<BoxDto> Boxes { get; set; }

    public BoxCollectionDto(string collectionId, List<BoxDto> boxes)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(boxes);
        CollectionId = collectionId;
        Boxes = boxes;
    }
}