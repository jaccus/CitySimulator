using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace City
{
    public partial class DistrictArchitectPage
    {
        private const int _polygonStrokeThickness = 2;
        private const int _polylineStrokeThickness = 5;
        private const double _polylineOpacity = 0.7;
        private const double _polygonOpacity = 0.7;
        private readonly Color _polygonFillColor = Colors.Green;
        private readonly Color _polygonStrokeColor = Colors.Green;
        private readonly Color _polygonStrokeColorSelected = Colors.Yellow;
        private static readonly Color _polylineStrokeColor = Colors.Orange;
        private MapLayer _polygonLayer;
        private MapLayer _lastLineLayer;
        private MapLayer _linesLayer;
        private MapDistrict _selectedPolygon;
        private Stack<Location> _lastLinePoints;
        private Point _lastPosition;
        private Location _lastLineEnd;
        private Location _lastLineStart;
        private const int InitialPopulation = 0;

        protected MapDistrict SelectedPolygon {
            get
            {
                return _selectedPolygon;
            }
            set
            {
                if (_selectedPolygon != null)
                    _selectedPolygon.Stroke = new SolidColorBrush(_polygonStrokeColor);

                _selectedPolygon = value;

                if(SelectedPolygon != null)
                    _selectedPolygon.Stroke = new SolidColorBrush(_polygonStrokeColorSelected);

                districtPopulationTextBox.Text = 
                    SelectedPolygon == null ? InitialPopulation.ToString() : SelectedPolygon.Population.ToString();
                districtNameTextBox.Text = 
                    SelectedPolygon == null ? string.Empty : SelectedPolygon.DistrictName;
                
                selectedPolygonGroupBox.Visibility = 
                    SelectedPolygon == null ? Visibility.Hidden : Visibility.Visible;
            } 
        }

        public string SelectedPolygonPopulation
        {
            get { return SelectedPolygon == null ? InitialPopulation.ToString() : SelectedPolygon.Population.ToString(); }
            set
            {
                if (SelectedPolygon != null)
                    SelectedPolygon.Population = !string.IsNullOrEmpty(value) ? int.Parse(value) : InitialPopulation;
            }
        }

        public string SelectedPolygonName
        {
            get { return SelectedPolygon == null ? string.Empty : SelectedPolygon.DistrictName; }
            set { if (SelectedPolygon != null) SelectedPolygon.DistrictName = value; }
        }

        public IEnumerable<MapDistrict> Polygons {
            get { return _polygonLayer.Children.Cast<MapDistrict>(); }
        }

        public DistrictArchitectPage()
        {
            InitializeComponent();
            navigatorControl.MapObject = MyMap;

            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(CommonUtilities.BingMapsKey);
            MyMap.MouseLeftButtonUp += MyMap_MouseLeftButtonUp;

            selectedPolygonGroupBox.Visibility = Visibility.Hidden;

            districtPopulationTextBox.DataContext = this;
            districtNameTextBox.DataContext = this;

            MyMap.MouseLeftButtonDown += MyMap_MouseLeftButtonDown;
            MyMap.MouseRightButtonDown += MyMapMouseRightButtonDown;

            // Disable double click
            MyMap.MouseDoubleClick += (sender, e) =>
                                          {
                                              if (e.ChangedButton == MouseButton.Right)
                                                  e.Handled = true;
                                          };
            MyMap.MouseMove += MyMap_MouseMove;

            InitializeMap();
        }

        private void MyMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            districtPopulationTextBox.MoveFocus(new TraversalRequest(new FocusNavigationDirection()));
            Focus();

            if (_polygonLayer.Children.Count <= 0) return;

            var polygons = _polygonLayer.Children.Cast<MapDistrict>();

            var location = MyMap.ViewportPointToLocation(e.GetPosition(MyMap));

            foreach (var polygon in polygons)
            {
                var pointInPolygon = GeoUtilities.IsPointInPolygon(polygon.Locations, location);

                if (pointInPolygon)
                {
                    SelectedPolygon = polygon;
                    return;
                }
            }
            SelectedPolygon = null;
        }

        private void InitializeMap()
        {
            _lastLineLayer = new MapLayer();
            _linesLayer = new MapLayer();
            _polygonLayer = new MapLayer();

            _lastLinePoints = new Stack<Location>();

            MyMap.Children.Add(_lastLineLayer);
            MyMap.Children.Add(_linesLayer);
            MyMap.Children.Add(_polygonLayer);

            KeyDown += KeyDownHandler;
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            Logger.LogInfo("key pressed: {0}", e.Key.ToString());
            if (e.Key == Key.Delete)
                DeleteSelectedPolygon();
        }

        private void DeleteSelectedPolygon()
        {
            if (MessageBox.Show("Delete selected district?",
                "Delete confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            _polygonLayer.Children.Remove(SelectedPolygon);
            SelectedPolygon = null;
        }

        private void MyMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_lastLinePoints.Count <= 3) return;

            SelectedPolygon = null;

            CreatePolygon();
        }

        private void CreatePolygon()
        {
            _lastLinePoints.Pop(); // exclude temporary point
            
            var locationCollection = new LocationCollection();
            
            foreach (var point in _lastLinePoints)
                locationCollection.Add(point);

            DrawPolygon(locationCollection, string.Empty, InitialPopulation);

            _linesLayer.Children.Clear();
            _lastLineLayer.Children.Clear();
            _lastLinePoints.Clear();
        }

        public void DrawPolygon(LocationCollection locationCollection, string districtName, int population)
        {
            var polygon = CreatePolygon(locationCollection, districtName, population);

            _polygonLayer.Children.Add(polygon);
        }

        private MapDistrict CreatePolygon(LocationCollection locationCollection, string districtName, int population)
        {
            var polygon = new MapDistrict
                              {
                                  DistrictName = districtName,
                                  Population = population,
                                  Fill = new SolidColorBrush(_polygonFillColor),
                                  Stroke = new SolidColorBrush(_polygonStrokeColor),
                                  StrokeThickness = _polygonStrokeThickness,
                                  Opacity = _polygonOpacity,
                                  Locations = locationCollection
                              };
            return polygon;
        }

        private static MapPolyline DrawLine(Location start, Location end)
        {
            var polyline = new MapPolyline
                                {
                                    Stroke = new SolidColorBrush(_polylineStrokeColor),
                                    StrokeThickness = _polylineStrokeThickness,
                                    Opacity = _polylineOpacity,
                                    Locations = new LocationCollection { start, end }
                                };
            return polyline;
        }

        private void MyMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (!HasDrawingStarted()) return;
            //Logger.LogInfo("MyMap_MouseMove");

            var position = e.GetPosition(MyMap);
            var x = position.X;
            var y = position.Y;
            var dx = Math.Abs(_lastPosition.X) - Math.Abs(x);
            var dy = Math.Abs(_lastPosition.Y) - Math.Abs(y);
            //Logger.LogInfo("\tx:{0} y:{1} dx:{2} dy:{3}", x, y, dx, dy);

            const int delta = 3;

            var movedNotMoreThanDelta = Math.Abs(dx) <= delta && Math.Abs(dy) <= delta;
            if (movedNotMoreThanDelta) return;

            //Logger.LogInfo("\tpoints count:{0}", _lastLinePoints.Count);

            //Logger.LogInfo("\tremoving position x:{0} y{1} from _lastLinePoints", _lastLinePoints.Peek().Latitude, _lastLinePoints.Peek().Longitude);
            _lastLinePoints.Pop();

            var lineStart = _lastLinePoints.Peek();

            //Logger.LogInfo("\tadding position x:{0} y{1} to _lastLinePoints", position.X, position.Y);
            _lastLinePoints.Push(MyMap.ViewportPointToLocation(position));

            var lineEnd = _lastLinePoints.Peek();

            UpdateLastLine(lineStart, lineEnd);

            _lastPosition = position;
        }

        private bool HasDrawingStarted()
        {
            return _lastLinePoints.Count > 0;
        }

        private void UpdateLastLine(Location lineStart, Location lineEnd)
        {
            _lastLineLayer.Children.Clear();

            var line = DrawLine(lineStart, lineEnd);

            _lastLineLayer.Children.Add(line);

            _lastLineStart = lineStart;
            _lastLineEnd = lineEnd;
        }

        private void MyMapMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Logger.LogInfo("MyMapMouseRightButtonDown");

            var position = e.GetPosition(MyMap);
            var latlong = MyMap.ViewportPointToLocation(position);

            if(!HasDrawingStarted())
            {
                Logger.LogInfo("\tinitial adding position x:{0} y{1} to _lastLinePoints", position.X, position.Y);
                _lastLinePoints.Push(latlong);
            }

            Logger.LogInfo("\tadding position x:{0} y{1} to _lastLinePoints", position.X, position.Y);
            _lastLinePoints.Push(latlong);

            if(_lastLineLayer.Children.Count > 0)
            {
                _lastLineLayer.Children.Clear();
                _linesLayer.Children.Add(DrawLine(_lastLineStart, _lastLineEnd));

                Logger.LogInfo("Line added.");
            }
            _lastPosition = position;
        }

        public void RemovePolygons()
        {
            _polygonLayer.Children.Clear();
        }

        private void DeleteBtnClick(object sender, RoutedEventArgs e)
        {
            DeleteSelectedPolygon();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CommonUtilities.IsNumberAllowed(e.Text);
        }
    }
}
