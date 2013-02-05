using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;

namespace City
{
    public partial class AreaSelectorPage : INotifyPropertyChanged
    {
        private List<CircleAreaObject> _areas;
        public List<CircleAreaObject> Areas
        {
            get { return _areas; }
            set
            {
                _areas = value;
                foreach (var area in _areas)
                    MyMap.Children.Add(area);

                NotifyPropertyChanged("SelectedAreaMsg");
            }
        }
        public CircleAreaObject CurrentArea { get; set; }

        private Point _lastPosition;

        public Point LastPosition
        {
            get { return _lastPosition; }
            set
            {
                _lastPosition = value;
                NotifyPropertyChanged("SelectedAreaMsg");
            }
        }

        public string SelectedAreaMsg
        {
            get
            {
                if (Areas.Count <= 0) 
                    return "Not selected.";

                var pluralSuffix = Areas.Count == 1 ? "" : "(s)";
                return string.Format("{0} POI area{1} selected.", Areas.Count, pluralSuffix);
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public AreaSelectorPage()
        {
            InitializeComponent();
            navigatorControl.MapObject = MyMap;
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(CommonUtilities.BingMapsKey);
            Areas = new List<CircleAreaObject>();

            MyMap.MouseRightButtonDown += RightButtonDown;
            MyMap.MouseMove += MouseMoved;

            // Disable double click
            MyMap.MouseDoubleClick += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Right)
                    e.Handled = true;
            };

            DisplayRemoveButtons(false);
        }

        private void DisplayRemoveButtons(bool display)
        {
            var action = display ? Visibility.Visible : Visibility.Hidden;

            removeLastAreaBtn.Visibility = action;
            removeAllAreasBtn.Visibility = action;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RemoveLastAreaBtnClick(object sender, RoutedEventArgs e)
        {
            CurrentArea = null;
            
            if (Areas.Count <= 0) return;

            var lastCircleArea = Areas[Areas.Count - 1];
            MyMap.Children.Remove(lastCircleArea);
            Areas.Remove(lastCircleArea);
            NotifyPropertyChanged("SelectedAreaMsg");

            if(Areas.Count == 0)
                DisplayRemoveButtons(false);
        }

        private void RemoveAllAreasBtnClick(object sender, RoutedEventArgs e)
        {
            CurrentArea = null;

            Areas.Clear();
            MyMap.Children.Clear();
            NotifyPropertyChanged("SelectedAreaMsg");

            DisplayRemoveButtons(false);
        }

        private void MouseMoved(object sender, MouseEventArgs e)
        {
            // Nothing clicked yet
            if (CurrentArea == null) return;

            // Pushpin set, adjusting circle mode

            var position = e.GetPosition(MyMap);

            var dx = Math.Abs(LastPosition.X) - Math.Abs(position.X);
            var dy = Math.Abs(LastPosition.Y) - Math.Abs(position.Y);

            const double delta = 1;

            var movedNotMoreThanDelta = Math.Abs(dx) <= delta && Math.Abs(dy) <= delta;
            if (movedNotMoreThanDelta) return;

            Point pushPinPoint;
            if (!MyMap.TryLocationToViewportPoint(CurrentArea.Pushpin.Location, out pushPinPoint))
                return;

            CurrentArea.Range = GeoUtilities.DirectDistance(
                MyMap.ViewportPointToLocation(e.GetPosition(MyMap)),
                MyMap.ViewportPointToLocation(pushPinPoint));

            LastPosition = position;
        }

        private void RightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(MyMap);
            var location = MyMap.ViewportPointToLocation(position);

            // No pushpin yet, left clicked = setting pushpin
            if (CurrentArea == null)
            {
                DrawCircleArea(location.Latitude, location.Longitude);

                LastPosition = position;

                DisplayRemoveButtons(true);
            }
            else //pushpin exists, confirming circle area click
            {
                CurrentArea = null;
            }
        }

        public void DrawCircleArea(double lat, double lng)
        {
            CurrentArea = new CircleAreaObject(lat, lng, 0);

            MyMap.Children.Add(CurrentArea);
            Areas.Add(CurrentArea);
            NotifyPropertyChanged("SelectedAreaMsg");
        }
    }
}