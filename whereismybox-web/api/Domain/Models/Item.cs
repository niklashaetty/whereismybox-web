using Newtonsoft.Json;

namespace Domain.Models;

public class Item
{
    [JsonProperty] public Guid ItemId { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    
    public static Item Create(string name, string description="")
    {
        ArgumentNullException.ThrowIfNull(name);
        return new Item(Guid.NewGuid(), name, description);
    }
    
    [JsonConstructor]
    protected Item()
    {
    }
    
    public Item(Guid itemId, string name, string description)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ItemId = itemId;
        Name = name;
        Description = description;
    }

}