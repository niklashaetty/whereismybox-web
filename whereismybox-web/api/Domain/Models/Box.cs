using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class Box
{
    [JsonProperty] public CollectionId CollectionId { get; private set; }
    [JsonProperty] public BoxId BoxId { get; private set; }
    [JsonProperty] public int Number { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public List<Item> Items { get; private set; }

    public static Box Create(CollectionId collectionId, BoxId boxId, int boxNumber, string boxName)
    {
        return new Box(collectionId, boxId, boxNumber, boxName, new List<Item>());
    }
    
    [JsonConstructor]
    protected Box()
    {
    }
    
    public Box(CollectionId collectionId, BoxId boxId, int number, string name, List<Item> items)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(boxId);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(items);
        CollectionId = collectionId;
        BoxId = boxId;
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
    
    public bool TryGetItem(ItemId itemId, out Item item)
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

    public bool RemoveItem(ItemId itemId)
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