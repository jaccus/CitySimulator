using System;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace City
{
    public class CircleAreaObject : MapLayer
    {
        private Pushpin _pushpin;
        public Pushpin Pushpin
        {
            get { return _pushpin; }
            set
            {
                if(_pushpin != null) 
                    Children.Remove(_pushpin);

                _pushpin = value;
                Children.Add(_pushpin);

                // Refresh range to redraw the circle
                Range = Range;

                // Set Lat/Lng values
                Latitude = _pushpin.Location.Latitude;
                Longitude = _pushpin.Location.Longitude;
            }
        }

        private MapPolyline _circle;

        private double _range;
        public double Range
        {
            get { return _range; }
            set
            {
                _range = value;

                if (_pushpin == null)
                    return;
                var circleCenter = _pushpin.Location;

                if (_circle != null)
                    Children.Remove(_circle);

                _circle = new MapPolyline
                {
                    Locations = CreateCircle(circleCenter, _range),
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Colors.Transparent),
                    FillRule = FillRule.EvenOdd
                };

                Children.Add(_circle);
            }
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public CircleAreaObject(double lat, double lng, double range)
        {
            Pushpin = new Pushpin { Location = new Location(lat, lng) };
            if (range != 0) Range = range;
        }

        public static double DegToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadToDeg(double radians)
        {
            return radians * (180 / Math.PI);
        }

        /// <summary>
        /// Creates the circle. Implemented after
        /// http://pietschsoft.com/post/2010/06/28/Silverlight-Bing-Maps-Draw-Circle-Around-Latitude-Longitude-Location.aspx
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static LocationCollection CreateCircle(Location center, double radius)
        {
            var lat = DegToRad(center.Latitude);
            var lng = DegToRad(center.Longitude);
            var d = radius / 6371.0;
            var locations = new LocationCollection();

            for (var x = 0; x <= 360; x++)
            {
                var brng = DegToRad(x);
                var cosD = Math.Cos(d);
                var sinD = Math.Sin(d);
                var cosLat = Math.Cos(lat);
                var sinLat = Math.Sin(lat);
                var latRad = Math.Asin(sinLat * cosD + cosLat * sinD * Math.Cos(brng));
                var lngRad = lng + Math.Atan2(Math.Sin(brng) * sinD * cosLat, cosD - sinLat * Math.Sin(latRad));

                locations.Add(new Location(RadToDeg(latRad), RadToDeg(lngRad)));
            }

            return locations;
        }
    }
}
