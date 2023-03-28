using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class UnattachedItemDto : ItemDto
{
    [OpenApiProperty(Description = "The box it was previously located in. Optional")]
    public Guid? PreviousBoxId { get; set; }

    [OpenApiProperty(Description = "The box number it was previously located in. Optional")]
    public int? PreviousBoxNumber { get; set; }

    public UnattachedItemDto(Guid itemId, string name, string description, Guid? previousBoxId, int? previousBoxNumber)
        : base(itemId, name, description)
    {
        PreviousBoxId = previousBoxId;
        PreviousBoxNumber = previousBoxNumber;
    }
}