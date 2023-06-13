using Domain.Models;
using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetCollectionContributorsQuery(Permissions Permissions, CollectionId CollectionId) : IQuery
{
    public Permissions Permissions { get; } = Permissions ?? throw new ArgumentNullException(nameof(Permissions));
    public CollectionId CollectionId { get; } = CollectionId ?? throw new ArgumentNullException(nameof(CollectionId));
}