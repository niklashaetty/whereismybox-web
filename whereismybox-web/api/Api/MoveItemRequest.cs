using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class MoveItemRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The identifier of the new box")]
    public string TargetBoxId { get; set; }
    

    public MoveItemRequest(string targetBoxId)
    {
        ArgumentNullException.ThrowIfNull(targetBoxId);
        TargetBoxId = targetBoxId;
    }
}