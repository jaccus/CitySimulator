using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Xml.Linq;

namespace City
{
    public static class CommonUtilities
    {
        static NumberFormatInfo _ni;

        public static string POIFile
        {
            get { return Properties.Settings.Default.poi_file; }
        }

        public static string BingMapsKey
        {
            get { return Properties.Settings.Default.bing_maps_key; }
        }

        public static string DecimalDelimiterToDot(double number)
        {
            return number.ToString().Replace(',', '.');
        }

        public static double ParseDouble(string str)
        {
            double d;
            if (!double.TryParse(str, out d))
            {
                var ci = CultureInfo.InstalledUICulture;
                _ni = (NumberFormatInfo)
                     ci.NumberFormat.Clone();
                _ni.NumberDecimalSeparator = ".";

                d = double.Parse(str, _ni);
            }
            return d;
        }

        public static IEnumerable<NavigationButton> LoadNavigationButtons()
        {
            var poiXml = Properties.Settings.Default.navigation_xml;

            var document = XDocument.Load(poiXml);
            var buttons = from c in document.Descendants("navigation")
                          select new NavigationButton
                          {
                              // ReSharper disable PossibleNullReferenceException
                              Latitude = double.Parse(c.Attribute("Latitude").Value, CultureInfo.InvariantCulture),
                              Longitude = double.Parse(c.Attribute("Longitude").Value, CultureInfo.InvariantCulture),
                              Zoom = double.Parse(c.Attribute("Zoom").Value, CultureInfo.InvariantCulture),
                              Content = c.Attribute("Label").Value
                              // ReSharper restore PossibleNullReferenceException
                          };
            return buttons;
        }

        public static bool IsNumberAllowed(string text)
        {
            var regex = new Regex("[0-9]+");
            return regex.IsMatch(text);
        }

        public static IEnumerable<string> ReadSqlCECommands(string sqlFile)
        {
            var tmpFile = Path.GetTempFileName();

            var allText = File.ReadAllText(sqlFile);
            var validPart = allText.Split(new[] {"Creating all tables"}, StringSplitOptions.None)[1];
            File.WriteAllText(tmpFile, validPart);

            var lines = File.ReadAllLines(tmpFile)
                .Where(line => 
                    !line.Trim().StartsWith("GO") && 
                    !line.Trim().StartsWith("--") && 
                    !string.IsNullOrEmpty(line.Trim()));

            var sb = new StringBuilder();
            
            foreach (var line in lines)
                sb.Append(line);

            var linesTogether = sb.ToString();

            var strings = linesTogether
                .Split(';')
                .Select(l => l + ';')
                .Where(l => l != ";");

            return strings;
        }
    }
}