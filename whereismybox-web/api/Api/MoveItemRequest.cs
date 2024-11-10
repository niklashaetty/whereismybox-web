using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class MoveItemRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The number of the new box")]
    public int TargetBoxNumber { get; set; }
    

    public MoveItemRequest(int targetBoxNumber)
    {
        TargetBoxNumber = targetBoxNumber;
    }
}