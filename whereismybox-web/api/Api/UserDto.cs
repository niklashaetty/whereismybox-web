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
    public string Username { get; set; }
    
    
    [JsonRequired]
    [OpenApiProperty(Description = "Indicates if the user has registered their username or is using a temporary one")]
    public bool IsRegistered { get; set; }
    
    public UserDto(Guid userId, string username, bool isRegistered)
    {
        ArgumentNullException.ThrowIfNull(username);
        UserId = userId;
        Username = username;
        IsRegistered = isRegistered;
    }
}