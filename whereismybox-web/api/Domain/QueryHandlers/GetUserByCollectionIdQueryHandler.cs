using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetUserByCollectionIdQueryHandler : IQueryHandler<GetUserByCollectionIdQuery, User>
{
    private readonly IUserRepository _userRepository;
    
    public GetUserByCollectionIdQueryHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(GetUserByCollectionIdQuery query)
    {
        return await _userRepository.Get(query.CollectionId);
    }
}