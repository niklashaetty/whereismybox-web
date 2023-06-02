using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class Item
{
    [JsonProperty] public ItemId ItemId { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }

    [JsonConstructor]
    protected Item()
    {
    }
    
    public Item(ItemId itemId, string name, string description)
    {
        ArgumentNullException.ThrowIfNull(itemId);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ItemId = itemId;
        Name = name;
        Description = description;
    }

}