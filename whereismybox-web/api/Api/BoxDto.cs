using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class BoxDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The box identifier")]
    public Guid BoxId { get; set; }

    [JsonRequired]
    [OpenApiProperty(Description = "The name of the box", Default = "My first box")]
    public string Name { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The box number. Must be unique per user", Default = 24)]
    public int Number { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The items in this box")]
    public List<ItemDto> Items { get; set; }

    public BoxDto(Guid boxId, string name, int number, List<ItemDto> items)
    {
        ArgumentNullException.ThrowIfNull(number);
        ArgumentNullException.ThrowIfNull(items);
        BoxId = boxId;
        Name = name;
        Number = number;
        Items = items;
    }
}