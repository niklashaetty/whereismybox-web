using System;
using System.Linq;
using Api;
using Domain.Models;

namespace Functions.Mappers;

public static class BoxMapper
{
    public static BoxDto ToApiModel(this Box box)
    {
        ArgumentNullException.ThrowIfNull(box);
        return new BoxDto(box.BoxId, box.Name, box.Number,
            box.Items.Select(i => i.ToApiModel()).ToList());
    }

    public static ItemDto ToApiModel(this Item item)
    {
        return new ItemDto(item.ItemId, item.Name, item.Description);
    }

    public static UnattachedItemDto ToApiModel(this UnattachedItem item)
    {
        return new UnattachedItemDto(item.ItemId, item.Name, item.Description, item.PreviousBoxId,
            item.PreviousBoxNumber);
    }
}