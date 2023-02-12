using Domain.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Infrastructure.UnattachedItemRepository;

public class CosmosAwareUnattachedItemCollection : UnattachedItemCollection, ICosmosAware
{
    [JsonProperty(PropertyName = "_etag")] 
    public string ETag { get; set; }
    
    [JsonProperty(PropertyName = "id")] 
    public string Id { get; set; } 
    
    public CosmosAwareUnattachedItemCollection(Guid userId, List<UnattachedItem> unattachedItems) : base(userId, unattachedItems)
    {
        Id = userId.ToString();
    }
    
    public static CosmosAwareUnattachedItemCollection ToCosmosAware(UnattachedItemCollection unattachedItemCollection)
    {
        ArgumentNullException.ThrowIfNull(unattachedItemCollection);
        return new CosmosAwareUnattachedItemCollection(unattachedItemCollection.UserId, unattachedItemCollection.UnattachedItems);
    }

    public PartitionKey GetPartitionKey()
    {
        return new PartitionKey(UserId.ToString());
    }
}