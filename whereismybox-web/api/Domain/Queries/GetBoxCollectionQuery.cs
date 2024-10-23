using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetBoxCollectionQuery(CollectionId CollectionId, UserId UserId) : IQuery
{
    public readonly UserId UserId = UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public readonly  CollectionId CollectionId = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}