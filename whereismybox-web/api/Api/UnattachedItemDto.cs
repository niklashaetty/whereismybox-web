using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UnattachedItemDto : ItemDto
{
    [OpenApiProperty(Description = "The item it was previously located in. Optional")]
    public Guid? PreviousBoxId { get; set; }

    public UnattachedItemDto(Guid itemId, Guid? previousBoxId, string name, string description) : base(itemId, name, description)
    {
        PreviousBoxId = previousBoxId;
    }
}