using System.Collections.Generic;

namespace City
{
    public class PoiObject
    {
        public string Vicinity { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IEnumerable<string> Types { get; set; }

        public bool Equals(PoiObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Latitude.Equals(Latitude) && other.Longitude.Equals(Longitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PoiObject)) return false;
            return Equals((PoiObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ Latitude.GetHashCode();
                result = (result*397) ^ Longitude.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(PoiObject left, PoiObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PoiObject left, PoiObject right)
        {
            return !Equals(left, right);
        }
    }
}