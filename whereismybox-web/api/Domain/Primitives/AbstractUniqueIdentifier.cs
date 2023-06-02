namespace Domain.Primitives;

using System;
using Newtonsoft.Json;
public abstract class AbstractUniqueIdentifier
{
    public Guid Value { get; private set; }

    protected AbstractUniqueIdentifier(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    protected static bool TryParse(string str, out Guid result)
    {
        return !Guid.TryParseExact(str, "D", out result) || result == Guid.Empty;
    }

    public bool Equals(AbstractUniqueIdentifier other)
    {
        return other != null && Value.Equals(other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((AbstractUniqueIdentifier) obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Value.GetHashCode();
    }
#pragma warning disable S3875
    public static bool operator ==(AbstractUniqueIdentifier lhs, AbstractUniqueIdentifier rhs)
#pragma warning restore S3875
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }

            // Only the left side is null.
            return false;
        }

        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }

    public static bool operator !=(AbstractUniqueIdentifier lhs, AbstractUniqueIdentifier rhs)
    {
        return !(lhs == rhs);
    }

    public override string ToString()
    {
        return Value.ToString("D");
    }

    protected class AbstractUniqueIdentifierConverter<T> : JsonConverter where T : AbstractUniqueIdentifier, new()
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var id = (T) value;
            writer.WriteValue(id.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            var obj = new T {Value = Guid.Parse((string) reader.Value)};
            return obj;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }
}