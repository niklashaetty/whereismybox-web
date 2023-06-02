using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class MoveUnattachedItemToBoxRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The box identifier")]
    public Guid BoxId { get; set; }

    public MoveUnattachedItemToBoxRequest(Guid boxId)
    {
        ArgumentNullException.ThrowIfNull(boxId);
        BoxId = boxId;
    }
}