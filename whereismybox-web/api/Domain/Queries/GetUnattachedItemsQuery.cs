using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetUnattachedItemsQuery(ExternalUserId ExternalUserId, CollectionId CollectionId) : IQuery
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId  ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}