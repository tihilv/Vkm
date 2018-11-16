using System;
using System.Diagnostics;

namespace Vkm.Api.Basic
{
    [Serializable]
    public struct Location
    {
        public bool Equals(Location other)
        {
            return X == other.X && Y == other.Y;
        }

        public readonly byte X;
        public readonly byte Y;

        public Location(int x, int y)
        {
            Debug.Assert(x >= 0 && y >= 0 && x < byte.MaxValue && y < byte.MaxValue, "Coordinates doesn't fit byte value");

            X = (byte)x;
            Y = (byte)y;
        }

        public static Location operator +(Location a, Location b)
        {
            return new Location(a.X + b.X, a.Y + b.Y);
        }

        public static bool operator ==(Location a, Location b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Location a, Location b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Location && Equals((Location) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }
    }
}
