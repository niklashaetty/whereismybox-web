using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UnattachedItemCollectionDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The unique identifier for a collection", Default = "2ss")]
    public string CollectionId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The unattached items for a collection")]
    public List<UnattachedItemDto> UnattachedItems { get; set; }

    public UnattachedItemCollectionDto(string collectionId, List<UnattachedItemDto> unattachedItems)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(unattachedItems);
        CollectionId = collectionId;
        UnattachedItems = unattachedItems;
    }
}