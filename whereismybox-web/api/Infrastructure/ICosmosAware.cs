using Newtonsoft.Json;

namespace Infrastructure;

public interface ICosmosAware
{
    [JsonProperty(PropertyName = "_etag")] 
    public string ETag { get; set; }
    
    [JsonProperty(PropertyName = "id")] 
    public string Id { get; set; } 
}