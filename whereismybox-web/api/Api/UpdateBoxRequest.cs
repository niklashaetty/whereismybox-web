using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UpdateBoxRequest
{
    [OpenApiProperty(Description = "The name of the box", Default = "My first box")]
    public string? Name { get; set; }
    
    [OpenApiProperty(Description = "The box number. Must be unique per user", Default = 24)]
    public int? Number { get; set; }

    public UpdateBoxRequest(string name, int? number)
    {
        Name = name;
        Number = number;
    }
}