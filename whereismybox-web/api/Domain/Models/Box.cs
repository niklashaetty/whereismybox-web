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

    /// <summary>
    /// Idempotently adds item
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true is item was added, false if it already exists</returns>
    public bool AddItem(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (Items.Any(i => i.ItemId == item.ItemId))
        {
            return false;
        }
        Items.Add(item);
        return true;
    }
    
    public bool TryGetItem(Guid itemId, out Item item)
    {
        var existingItem =  Items.FirstOrDefault(i => i.ItemId == itemId);
        if (existingItem is null)
        {
            item = null;
            return false;
        }

        item = existingItem;
        return true;
    }

    public bool RemoveItem(Guid itemId)
    {
        var itemsRemoved = Items.RemoveAll(i => i.ItemId == itemId);
        return itemsRemoved > 0;
    }

    public void UpdateItem(Item item)
    {
        var index = Items.FindIndex(i => i.ItemId == item.ItemId);
        if (index == -1)
        {
            throw new InvalidOperationException($"Item {item.ItemId} not found");
        }

        Items[index] = item;
    }
}