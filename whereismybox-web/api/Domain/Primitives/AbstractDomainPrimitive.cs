using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Domain.Primitives;

public abstract class AbstractDomainPrimitive<T> : IEquatable<AbstractDomainPrimitive<T>>
{
    public bool Equals(AbstractDomainPrimitive<T> other)
    {
        return other != null && Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
        return obj is AbstractDomainPrimitive<T> domainPrimitive && Equals(domainPrimitive);
    }

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

    public override string ToString() => Value.ToString();

    public abstract T Value { get; protected set; }

    protected class DomainPrimitiveJsonConverter<T1> : JsonConverter where T1 : AbstractDomainPrimitive<T>, new()
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is T1 id && id.Value != null)
            {
                writer.WriteValue(id.Value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            JToken.Load(reader);
            return new T1 { Value = reader.Value is T value ? value : default };
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(T);
    }
}