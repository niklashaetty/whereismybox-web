using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class Collection
{
    [JsonProperty] public CollectionId CollectionId { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    
    [JsonProperty] public UserId Owner { get; private set; }
    [JsonProperty] public List<UserId> Contributors { get; private set; }

    public static Collection Create(CollectionId collectionId, string name, UserId owner)
    {
        return new Collection(collectionId, name, owner, new List<UserId>());
    }

    public Collection(CollectionId collectionId, string name, UserId owner, List<UserId> contributors)
    {
        CollectionId = collectionId;
        Name = name;
        Owner = owner;
        Contributors = contributors;
    }

    [JsonConstructor]
    protected Collection()
    {
    }
    
    public void AddContributor(User user)
    {
        if (Contributors.Any(u => u.Equals(user.UserId)) is false)
        {
            Contributors.Add(user.UserId);
        }
    }
    
    public void RemoveAsContributor(UserId userId)
    {
        Contributors.RemoveAll(u => u.Equals(userId));
    }
}