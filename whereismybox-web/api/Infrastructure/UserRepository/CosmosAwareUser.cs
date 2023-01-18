using Domain.Models;
using Newtonsoft.Json;

namespace Infrastructure.UserRepository;

public class CosmosAwareUser : User
{
    [JsonProperty(PropertyName = "_etag")] 
    public string ETag { get; set; }
    
    [JsonProperty(PropertyName = "id")] 
    public string Id { get; set; } 
    
    public CosmosAwareUser(Guid userId, string userName) : base(userId, userName)
    {
        Id = userId.ToString();
    }

    public static CosmosAwareUser ToCosmosAware(User user)
    {
        return new CosmosAwareUser(user.UserId, user.UserName);
    }
}