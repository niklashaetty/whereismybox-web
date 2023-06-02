using System.Text;
using Newtonsoft.Json;

namespace Domain.Primitives;

[JsonConverter(typeof(DomainPrimitiveJsonConverter<CollectionId>))]
public class CollectionId : AbstractDomainPrimitive<string>
{
    public sealed override string Value { get; protected set; }
    private const int Length = 7;
    private static readonly char[] Base62Chars = "23456789abcdefghjkmnpqrstuvwxyz".ToCharArray();

    [JsonConstructor]
    public CollectionId(string collectionId)
    {
        if (IsValid(collectionId) is false)
        {
            throw new ArgumentException(collectionId);
        }
        Value = collectionId;
    }
    
    public CollectionId()
    {
    }

    public static bool IsValid(string value)
    {
        if (value.Length != Length)
        {
            return false;
        }

        if (value.Any(letter => Base62Chars.Contains(letter) is false))
        {
            return false;
        }

        return true;
    }

    public static bool TryParse(string value, out CollectionId collectionId)
    {
        if (IsValid(value))
        {
            collectionId = new CollectionId(value);
            return true;
        }

        collectionId = default!;
        return false;
    }
    
    public static CollectionId GenerateNew()
    {
        var random = new Random();
        var sb = new StringBuilder();
        for (var i=0; i<Length; i++) 
            sb.Append(Base62Chars[random.Next(31)]);

        return new CollectionId(sb.ToString());
    }
}