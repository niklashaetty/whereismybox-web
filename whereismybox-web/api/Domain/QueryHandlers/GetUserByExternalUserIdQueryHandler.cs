using Domain.Authorization;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetUserByExternalUserIdQueryHandler : IQueryHandler<GetUserByExternalUserIdQuery, User>
{
    private readonly IUserRepository _userRepository;
    
    public GetUserByExternalUserIdQueryHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(GetUserByExternalUserIdQuery query)
    {
        return await _userRepository.Get(query.ExternalUserId);
    }
}