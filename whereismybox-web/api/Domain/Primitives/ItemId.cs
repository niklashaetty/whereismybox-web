using Newtonsoft.Json;

namespace Domain.Primitives;

[JsonConverter(typeof(AbstractUniqueIdentifierConverter<ItemId>))]
public class ItemId : AbstractUniqueIdentifier
{
    
    public ItemId() : base(Guid.NewGuid())
    {
    }
    
    public ItemId(Guid guid) : base(guid)
    {
    }

    public static bool TryParse(string str, out ItemId result)
    {
        if (AbstractUniqueIdentifier.TryParse(str, out var guid))
        {
            result = null;
            return false;
        }

        result = new ItemId(guid);
        return true;
    }
}