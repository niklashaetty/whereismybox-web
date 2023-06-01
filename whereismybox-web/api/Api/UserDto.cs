using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UserDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The user identifier")]
    public Guid UserId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The initial collectionId that is given to the user. base32 string of 7 char", Default = "abcde12")]
    public string PrimaryCollectionId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The name of the user.", 
        Default = "John Doe")]
    public string UserName { get; set; }

    public UserDto(Guid userId, string userName, string primaryCollectionId)
    {
        ArgumentNullException.ThrowIfNull(userName);
        ArgumentNullException.ThrowIfNull(primaryCollectionId);
        UserId = userId;
        UserName = userName;
        PrimaryCollectionId = primaryCollectionId;
    }
}