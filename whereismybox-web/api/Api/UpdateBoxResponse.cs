using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UpdateBoxResponse
{
    [JsonRequired]
    [OpenApiProperty(Description = "The unique identifier for a collection", Default = "2ss21ab")]
    public string CollectionId { get; set; }
    
    [JsonRequired]
    [OpenApiProperty(Description = "The box identifier")]
    public Guid BoxId { get; set; }
    
    [OpenApiProperty(Description = "The box name (if it was changed")]
    public string Name  { get; set; }
    
    [OpenApiProperty(Description = "The box number (if it was changed)")]
    public int? Number { get; set; }
    
    public UpdateBoxResponse(string collectionId, Guid boxId, string? name, int? number)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        CollectionId = collectionId;
        BoxId = boxId;
        Name = name;
        Number = number;
    }
}