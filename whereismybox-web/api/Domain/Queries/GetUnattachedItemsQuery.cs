using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetUnattachedItemsQuery(CollectionId CollectionId) : IQuery
{
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}