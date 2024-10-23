using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetUnattachedItemsQuery(UserId UserId, CollectionId CollectionId) : IQuery
{
    public readonly UserId UserId = UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}