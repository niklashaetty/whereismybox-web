using System;
using System.Linq;
using Api;
using Domain.Models;

namespace Functions.Mappers;

public static class UserMapper
{
    public static UserDto ToApiModel(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return new UserDto(user.UserId.Value, user.Username, user.PrimaryCollectionId.Value,
            user.ContributorCollections.Select(c => c.Value).ToList());
    }

    public static CollectionContributor ToApiCollectionContributor(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return new CollectionContributor(user.UserId.Value, user.Username);
    }
}