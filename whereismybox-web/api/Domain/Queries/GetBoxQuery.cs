using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetBoxQuery(ExternalUserId ExternalUserId, CollectionId CollectionId, BoxId BoxId) : IQuery
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId  ?? throw new ArgumentNullException(nameof(ExternalUserId));
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
    public BoxId BoxId { get; } = BoxId ?? throw new ArgumentNullException(nameof(BoxId));
}