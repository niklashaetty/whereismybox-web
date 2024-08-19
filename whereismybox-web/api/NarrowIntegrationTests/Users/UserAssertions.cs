using System.Diagnostics;
using Api;
using Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace NarrowIntegrationTests.Users;

public static class UserAssertions
{
    public static string AssertRoleWithUserIdReturned(IActionResult assignUserRolesResponse)
    {
        ResponseAssertions.AssertSuccessStatusCode(assignUserRolesResponse);
        var roles = assignUserRolesResponse.GetContentOfType<RolesResponse>();
        var userIdRole = roles.Roles.FirstOrDefault();
        Assert.NotNull(userIdRole);
        
        Assert.StartsWith("userId.", userIdRole);
        return userIdRole![7..];
    }

    public static UserDto AssertUnregisteredUser(IActionResult userDtoResult, string expectedUserId)
    {
        ResponseAssertions.AssertSuccessStatusCode(userDtoResult);
        var userDto = userDtoResult.GetContentOfType<UserDto>();
        Assert.Equal(userDto.UserId.ToString(), expectedUserId);
        Assert.False(userDto.IsRegistered);
        return userDto;
    }
    
    public static UserDto AssertRegisteredUser(IActionResult userDtoResult, string expectedUserId)
    {
        ResponseAssertions.AssertSuccessStatusCode(userDtoResult);
        var userDto = userDtoResult.GetContentOfType<UserDto>();
        Assert.Equal(userDto.UserId.ToString(), expectedUserId);
        Assert.True(userDto.IsRegistered);
        return userDto;
    }
}