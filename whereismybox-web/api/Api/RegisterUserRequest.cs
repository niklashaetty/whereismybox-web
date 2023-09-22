using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Api;

public class RegisterUserRequest
{
    [OpenApiProperty(Description = "The name of the user", 
        Default = "John Doe")]
    public string UserName { get; set; }

    public RegisterUserRequest(string userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        UserName = userName;
    }
}