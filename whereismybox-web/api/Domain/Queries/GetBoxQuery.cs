using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetBoxQuery(CollectionId CollectionId, BoxId BoxId) : IQuery
{
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public BoxId BoxId { get; } = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
}