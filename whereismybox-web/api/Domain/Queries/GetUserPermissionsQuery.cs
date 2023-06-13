using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetUserPermissionsQuery(UserId UserId) : IQuery
{
    public UserId UserId { get; } = UserId ?? throw new ArgumentNullException(nameof(UserId));
}