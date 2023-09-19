using System;
using System.Linq;
using Api;
using Domain.Models;

namespace Functions.Mappers;

public static class CollectionMapper
{
    public static CollectionDto ToApiModel(this Collection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        return new CollectionDto(collection.CollectionId.Value, collection.Name, collection.Owner.Value,
            collection.Contributors.Select(u => u.Value).ToList());
    }
}