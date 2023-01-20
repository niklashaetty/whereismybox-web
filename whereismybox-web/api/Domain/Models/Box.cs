using Newtonsoft.Json;

namespace Domain.Models;

public class Box
{
    [JsonProperty] public Guid BoxId { get; private set; }
    [JsonProperty] public Guid UserId { get; private set; }
    [JsonProperty] public int Number { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public List<Item> Items { get; private set; }

    public static Box Create(Guid userId, int boxNumber, string boxName)
    {
        var boxId = Guid.NewGuid();
        return new Box(boxId, userId, boxNumber, boxName, new List<Item>());
    }
    
    [JsonConstructor]
    protected Box()
    {
    }
    
    public Box(Guid boxId, Guid userId, int number, string name, List<Item> items)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(items);
        BoxId = boxId;
        UserId = userId;
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