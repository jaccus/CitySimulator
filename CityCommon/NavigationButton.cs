using System.Windows.Controls;

namespace City
{
    public class NavigationButton : Button
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Zoom { get; set; }
    }
}