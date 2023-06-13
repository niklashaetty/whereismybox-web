using Domain.Primitives;

namespace Domain.QueryHandlers;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery
{
    Task<TResult> Handle(TQuery query);
}

public interface IQuery
{
}