using Domain.Exceptions;
using Domain.Models;
using Domain.Queries;
using Domain.Repositories;

namespace Domain.QueryHandlers;

public class GetUserPermissionsQueryHandler : IQueryHandler<GetUserPermissionsQuery, Permissions>
{
    private readonly IUserRepository _userRepository;
    
    public GetUserPermissionsQueryHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }
    
    public async Task<Permissions> Handle(GetUserPermissionsQuery query)
    {
        var existingUser = await _userRepository.Get(query.UserId);
        if (existingUser is null)
        {
            throw new UserNotFoundException(query.UserId);
        }

        return new Permissions(query.UserId, existingUser.PrimaryCollectionId, existingUser.ContributorCollections);
    }
}