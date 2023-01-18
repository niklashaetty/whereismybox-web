using Newtonsoft.Json;

namespace Domain.Models;

public class User
{
    [JsonProperty] public Guid UserId { get; private set; }
    [JsonProperty] public string UserName { get; private set; }

    public static User Create(string userName)
    {
        return new User(Guid.NewGuid(), userName);
    }
    
    [JsonConstructor]
    protected User()
    {
    }
    
    public User(Guid userId, string userName)
    {
        ArgumentNullException.ThrowIfNull(userName);
        UserId = userId;
        UserName = userName;
    }
    
}