using Newtonsoft.Json;

namespace Domain.Models;

public class UnattachedItem : Item
{
    [JsonProperty] public Guid? PreviousBoxId { get; private set; }
    [JsonProperty] public int? PreviousBoxNumber { get; private set; }
    
        
    [JsonConstructor]
    protected UnattachedItem()
    {
    }

    public UnattachedItem(Guid itemId, string name, string description, Guid previousBoxId, int previousBoxNumber) : 
        base(itemId, name, description)
    {
        PreviousBoxId = previousBoxId;
        PreviousBoxNumber = previousBoxNumber;
    }
    
    public UnattachedItem(Item item, Guid? previousBoxId) : 
        base(item.ItemId, item.Name, item.Description)
    {
        PreviousBoxId = previousBoxId;
    }

    public void AddPreviousBoxNumber(int previousBoxNumber)
    {
        PreviousBoxNumber = previousBoxNumber;
    }
    
    public void RemovePreviousBox()
    {
        PreviousBoxNumber = null;
        PreviousBoxNumber = null;
    }
}