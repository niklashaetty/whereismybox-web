using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class CreateCollectionRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The name of the collection", Default = "Basement")]
    public string Name { get; set; }

    public CreateCollectionRequest(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }
}