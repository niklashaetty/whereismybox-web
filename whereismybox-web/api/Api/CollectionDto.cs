using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class CollectionDto
{
    [JsonRequired]
    [OpenApiProperty(Description = "The collection Id")]
    public string CollectionId { get; set; }

    [JsonRequired]
    [OpenApiProperty(Description = "The collection name", Default = "Basement")]
    public string Name { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The userId of the owner of the collection")]
    public Guid Owner { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "List of users that are contributors to the collection")]
    public List<Guid> Contributors { get; set; }

    public CollectionDto(string collectionId, string name, Guid owner, List<Guid> contributors)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(contributors);
        CollectionId = collectionId;
        Name = name;
        Owner = owner;
        Contributors = contributors;
    }
}