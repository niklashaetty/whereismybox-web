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
            box.Items.Select(i => new ItemDto(i.ItemId, i.Name, i.Description)).ToList());
    }
}