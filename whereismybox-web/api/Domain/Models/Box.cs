using Newtonsoft.Json;

namespace Domain.Models;

public class Box
{
    [JsonProperty] public Guid BoxId { get; private set; }
    [JsonProperty] public int Number { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public List<Item> Items { get; private set; }

    public static Box Create(int boxNumber, string boxName)
    {
        return new Box(Guid.NewGuid(), boxNumber, boxName, new List<Item>());
    }
    
    [JsonConstructor]
    protected Box()
    {
    }
    
    public Box(Guid boxId, int number, string name, List<Item> items)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(items);
        BoxId = boxId;
        Number = number;
        Name = name;
        Items = items;
    }

    public void AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (Items.Any(i => i.ItemId == item.ItemId))
        {
            throw new InvalidOperationException("item already exists");
        }
        Items.Add(item);
    }

    public bool RemoveItem(Guid itemId)
    {
        var itemsRemoved = Items.RemoveAll(i => i.ItemId == itemId);
        return itemsRemoved > 0;
    }
}