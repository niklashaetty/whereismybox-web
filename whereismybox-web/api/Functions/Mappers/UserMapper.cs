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
        return new UserDto(user.UserId, user.UserName);
    }
}