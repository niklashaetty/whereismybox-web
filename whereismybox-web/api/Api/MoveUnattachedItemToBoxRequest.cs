using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class MoveUnattachedItemToBoxRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The box number")]
    public int BoxNumber { get; set; }

    public MoveUnattachedItemToBoxRequest(int boxNumber)
    {
        BoxNumber = boxNumber;
    }
}