using Domain.Models;
using Newtonsoft.Json;

namespace Infrastructure.BoxRepository;

public class CosmosAwareBox : Box
{
    [JsonProperty(PropertyName = "_etag")] 
    public string ETag { get; set; }
    
    [JsonProperty(PropertyName = "id")] 
    public string Id { get; set; } 
    
    public string PartitionKey { get; set; }
    
    public CosmosAwareBox(Guid boxId, Guid userId, int number, string name, List<Item> items) : base(boxId, userId, number, name, items)
    {
        Id = boxId.ToString();
        PartitionKey = userId.ToString();
    }

    public static CosmosAwareBox ToCosmosAware(Box box)
    {
        return new CosmosAwareBox(box.BoxId, box.UserId, box.Number, box.Name, box.Items);
    }
}