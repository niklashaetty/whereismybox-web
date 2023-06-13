using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class CollectionContributor
{
    [JsonRequired]
    [OpenApiProperty(Description = "The user identifier")]
    public Guid UserId { get; set; }

    [JsonRequired]
    [OpenApiProperty(Description = "The name of the user.", 
        Default = "John Doe")]
    public string Username { get; set; }

    public CollectionContributor(Guid userId, string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        UserId = userId;
        Username = username;
    }
}