using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class ItemDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The item identifier")]
    public Guid ItemId { get; set; }

    [JsonRequired]
    [OpenApiProperty(Description = "The name of the item", Default = "Ski gloves")]
    public string Name { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "Detailed description of the item", Default = "Red ones")]
    public string Description { get; set; }

    public ItemDto(Guid itemId, string name, string description)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ItemId = itemId;
        Name = name;
        Description = description;
    }
}