using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetUserQuery(UserId UserId) : IQuery
{
    public UserId UserId { get; } = UserId ?? throw new ArgumentNullException(nameof(UserId));
}