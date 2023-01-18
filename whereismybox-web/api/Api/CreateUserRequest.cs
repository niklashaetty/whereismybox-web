using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Api;

public class CreateUserRequest
{
    [OpenApiProperty(Description = "The name of the user", 
        Default = "John Doe")]
    public string UserName { get; set; }
}