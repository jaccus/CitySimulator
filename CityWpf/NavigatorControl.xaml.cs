using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;

namespace City
{
    /// <summary>
    /// Interaction logic for NavigatorControl.xaml
    /// </summary>
    public partial class NavigatorControl
    {
        public static readonly DependencyProperty MapObjectProperty = DependencyProperty.Register("MapObject", typeof(Map), typeof(NavigatorControl));

        public Map MapObject
        {
            get { return (Map)GetValue(MapObjectProperty); }
            set { SetValue(MapObjectProperty, value); }
        }

        public NavigatorControl()
        {
            InitializeComponent();
            DataContext = this;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            var navigationButtons = CommonUtilities.LoadNavigationButtons();

            foreach (var navigationButton in navigationButtons)
            {
                navigationButton.Click += GoToBtnClick;
                navigationButton.HorizontalContentAlignment = HorizontalAlignment.Center;

                navigationButtonsPanel.Children.Add(navigationButton);
            }
        }

        private void GoToBtnClick(object sender, RoutedEventArgs e)
        {
            var s = sender as NavigationButton;
            if (s == null) return;

            var center = new Location(s.Latitude, s.Longitude);
            var zoomLevel = s.Zoom;

            MapObject.SetView(center, zoomLevel);
        }
    }
}
