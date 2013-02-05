using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maps.MapControl.WPF;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace City
{
    public class GeoUtilities
    {
        const int EarthRadius = 6371;

        public static LocationCollection MapPointsToLocations(IEnumerable<MapPoint> mapPoints)
        {
            var locations = new LocationCollection();

            foreach (var mapPoint in mapPoints)
            {
                locations.Add(new Location(mapPoint.Lat, mapPoint.Lng));
            }
            return locations;
        }

        public static Location LocationFromMapPoint(MapPoint mapPoint)
        {
            return new Location(mapPoint.Lat, mapPoint.Lng);
        }

        public static GeoRectangle PolygonBorder(IList<Location> polygonPoints)
        {
            var minLat = polygonPoints.Min(p => p.Latitude);
            var maxLat = polygonPoints.Max(p => p.Latitude);
            var minLng = polygonPoints.Min(p => p.Longitude);
            var maxLng = polygonPoints.Max(p => p.Longitude);

            var rectangle = new GeoRectangle(minLat, maxLat, minLng, maxLng);
            return rectangle;
        }

        public static Location RandomLocationWithinPolygon(IList<Location> polygonPoints)
        {
            if (polygonPoints.Count < 3)
                return null;
            
            var border = PolygonBorder(polygonPoints);

            var random = new Random();

            while (true)
            {
                var randomLng = random.NextDouble()*(border.MaxLng - border.MinLng) + border.MinLng;
                var randomLat = random.NextDouble()*(border.MaxLat - border.MinLat) + border.MinLat;
                var randomLocation = new Location(randomLat, randomLng);

                if (IsPointInPolygon(polygonPoints, randomLocation))
                {
                    return randomLocation;
                }
            }
        }

        /*
         * Distance using the Haversine formula.
         */
        public static double DirectDistance(Location p, Location q)
        {
            
            var qLatRad = q.Latitude * (Math.PI / 180);
            var pLatRad = p.Latitude * (Math.PI / 180);

            var deltaLat = (q.Latitude - p.Latitude) * (Math.PI / 180);
            var deltaLng = (q.Longitude - p.Longitude) * (Math.PI / 180);

            var a = Math.Pow(Math.Sin(deltaLat / 2), 2) +
                    Math.Cos(qLatRad) * Math.Cos(pLatRad) * Math.Pow(Math.Sin(deltaLng / 2), 2);
            //var c = 2 * Math.Asin(Math.Sqrt(a));
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = c * EarthRadius;

            return d;
        }

        /*
         * Implemented after http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
         */
        public static bool IsPointInPolygon(IList<Location> polygonVertices, Location point)
        {
            var loc = polygonVertices;

            int i, j;
            var isInside = false;
            for (i = 0, j = loc.Count - 1; i < loc.Count; j = i++)
            {
                if (
                    (
                        (loc.ElementAt(i).Longitude > point.Longitude) != (loc.ElementAt(j).Longitude > point.Longitude)
                    )
                    &&
                    (
                        point.Latitude < (loc.ElementAt(j).Latitude - loc.ElementAt(i).Latitude)
                                *
                                (point.Longitude - loc.ElementAt(i).Longitude)
                        /
                        (loc.ElementAt(j).Longitude - loc.ElementAt(i).Longitude)
                        +
                        loc.ElementAt(i).Latitude
                    )
                    )
                    isInside = !isInside;
            }
            return isInside;
        }
    }
}