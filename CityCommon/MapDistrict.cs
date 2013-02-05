using Microsoft.Maps.MapControl.WPF;

namespace City
{
    public class MapDistrict : MapPolygon
    {
        public string DistrictName { get; set; }
        public int Population { get; set; }
    }
}