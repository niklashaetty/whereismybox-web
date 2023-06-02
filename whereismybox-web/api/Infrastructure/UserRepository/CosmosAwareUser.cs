using Domain.Primitives;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using User = Domain.Models.User;

namespace Infrastructure.UserRepository;

public class CosmosAwareUser : User
{
    [JsonProperty(PropertyName = "_etag")] public string ETag { get; set; }

    [JsonProperty(PropertyName = "id")] public string Id { get; set; }

    public CosmosAwareUser(UserId userId, string userName, CollectionId primaryCollectionId) : base(userId, userName,
        primaryCollectionId)
    {
        Id = userId.ToString();
    }

    public static CosmosAwareUser ToCosmosAware(User user)
    {
        return new CosmosAwareUser(user.UserId, user.UserName, user.PrimaryCollectionId);
    }

    public PartitionKey GetPartitionKey()
    {
        return new PartitionKey(UserId.ToString());
    }
}