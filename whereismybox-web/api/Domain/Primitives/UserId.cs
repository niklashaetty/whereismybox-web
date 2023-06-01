using Newtonsoft.Json;

namespace Domain.Primitives;

[JsonConverter(typeof(AbstractUniqueIdentifierConverter<UserId>))]
public class UserId : AbstractUniqueIdentifier
{
    
    public UserId() : base(Guid.NewGuid())
    {
    }
    
    public UserId(Guid guid) : base(guid)
    {
    }

    public static bool TryParse(string str, out UserId result)
    {
        if (AbstractUniqueIdentifier.TryParse(str, out var guid))
        {
            result = null;
            return false;
        }

        result = new UserId(guid);
        return true;
    }
}