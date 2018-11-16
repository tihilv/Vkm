using System;

namespace Vkm.Api.Identification
{
    [Serializable]
    public struct Identifier
    {
        public readonly string Value;

        public Identifier(string value)
        {
            Value = value;
        }

        public bool Equals(Identifier other)
        {
            return string.Equals(Value, other.Value);
        }

        public static bool operator ==(Identifier a, Identifier b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Identifier a, Identifier b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Identifier && Equals((Identifier) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}