using Newtonsoft.Json;

namespace Domain.Models;

public class UnattachedItem : Item
{
    [JsonProperty] public Guid? PreviousBoxId { get; private set; }
    
        
    [JsonConstructor]
    protected UnattachedItem()
    {
    }

    public UnattachedItem(Guid itemId, string name, string description, Guid previousBoxId) : 
        base(itemId, name, description)
    {
        PreviousBoxId = previousBoxId;
    }
    
    public UnattachedItem(Item item, Guid? previousBoxId) : 
        base(item.ItemId, item.Name, item.Description)
    {
        PreviousBoxId = previousBoxId;
    }
}