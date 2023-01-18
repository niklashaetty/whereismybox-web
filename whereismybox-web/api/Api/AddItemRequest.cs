using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class AddItemRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The name of the item", Default = "Ski gloves")]
    public string Name { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "Detailed description of the item", Default = "Red")]
    public string Description { get; set; }
}