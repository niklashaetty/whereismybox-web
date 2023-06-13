using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Api;

public class AddContributorRequest
{
    [JsonRequired]
    [OpenApiProperty(Description = "The username of the new contributor", Default = "JackTheContributor")]
    public string Username { get; set; }
    
    public AddContributorRequest(string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        Username = username;
    }
}