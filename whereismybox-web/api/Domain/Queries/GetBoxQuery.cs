using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetBoxQuery(UserId UserId, CollectionId CollectionId, BoxId BoxId) : IQuery
{
    public readonly UserId UserId = UserId  ?? throw new ArgumentNullException(nameof(UserId));
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public BoxId BoxId { get; } = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
}