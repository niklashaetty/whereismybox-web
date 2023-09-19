using Domain.Models;
using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetContributorCollectionsQuery(Permissions Permissions, UserId UserId) : IQuery
{
    public Permissions Permissions { get; } = Permissions ?? throw new ArgumentNullException(nameof(Permissions));
    public UserId UserId { get; } = UserId ?? throw new ArgumentNullException(nameof(UserId));
}