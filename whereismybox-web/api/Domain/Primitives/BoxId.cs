using Newtonsoft.Json;

namespace Domain.Primitives;

[JsonConverter(typeof(AbstractUniqueIdentifierConverter<BoxId>))]
public class BoxId : AbstractUniqueIdentifier
{
    
    public BoxId() : base(Guid.NewGuid())
    {
    }
    
    public BoxId(Guid guid) : base(guid)
    {
    }

    public static bool TryParse(string str, out BoxId result)
    {
        if (AbstractUniqueIdentifier.TryParse(str, out var guid))
        {
            result = null;
            return false;
        }

        result = new BoxId(guid);
        return true;
    }
}