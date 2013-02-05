namespace City
{
    public class GeoRectangle
    {
        public double MinLat { get; set; }
        public double MaxLat { get; set; }
        public double MinLng { get; set; }
        public double MaxLng { get; set; }

        public GeoRectangle(double minLat, double maxLat, double minLng, double maxLng)
        {
            MinLat = minLat;
            MaxLat = maxLat;
            MinLng = minLng;
            MaxLng = maxLng;
        }

        public bool Equals(GeoRectangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.MinLat.Equals(MinLat) && other.MaxLat.Equals(MaxLat) && other.MinLng.Equals(MinLng) && other.MaxLng.Equals(MaxLng);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GeoRectangle)) return false;
            return Equals((GeoRectangle) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = MinLat.GetHashCode();
                result = (result*397) ^ MaxLat.GetHashCode();
                result = (result*397) ^ MinLng.GetHashCode();
                result = (result*397) ^ MaxLng.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(GeoRectangle left, GeoRectangle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GeoRectangle left, GeoRectangle right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("MinLat: {0}, MaxLat: {1}, MinLng: {2}, MaxLng: {3}", MinLat, MaxLat, MinLng, MaxLng);
        }
    }
}