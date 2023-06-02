using Domain.Primitives;
using Domain.QueryHandlers;

namespace Domain.Queries;

public record GetUserByExternalUserIdQuery(ExternalUserId ExternalUserId) : IQuery
{
    public ExternalUserId ExternalUserId { get; } = ExternalUserId ?? throw new ArgumentNullException(nameof(ExternalUserId));
}