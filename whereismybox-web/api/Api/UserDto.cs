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
    public string Username { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "List of collectionIds that are shared with the user")]
    public List<string> ContributorCollections { get; set; }

    public UserDto(Guid userId, string username, string primaryCollectionId, List<string> contributorCollections)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(primaryCollectionId);
        ArgumentNullException.ThrowIfNull(contributorCollections);
        UserId = userId;
        Username = username;
        PrimaryCollectionId = primaryCollectionId;
        ContributorCollections = contributorCollections;
    }
}