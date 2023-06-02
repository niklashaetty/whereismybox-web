using Newtonsoft.Json;

namespace Domain.Primitives;

[JsonConverter(typeof(DomainPrimitiveJsonConverter<ExternalUserId>))]
public class ExternalUserId : AbstractDomainPrimitive<string>
{
    public sealed override string Value { get; protected set; }
   
    [JsonConstructor]
    public ExternalUserId(string externalUserId)
    {
        Value = externalUserId;
    }
    
    public ExternalUserId()
    {
    }
}