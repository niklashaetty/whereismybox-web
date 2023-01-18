using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UserDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The user identifier")]
    public Guid UserId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The name of the user.", 
        Default = "John Doe")]
    public string UserName { get; set; }

    public UserDto(Guid userId, string userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        UserId = userId;
        UserName = userName;
    }
}