using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class CreateBoxResponse
{
    [JsonRequired]
    [OpenApiProperty(Description = "The unique identifier for a collection", Default = "2ss21ab")]
    public string CollectionId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The box identifier")]
    public Guid BoxId { get; set; }

    public CreateBoxResponse(string collectionId, Guid boxId)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        CollectionId = collectionId;
        BoxId = boxId;
    }
}