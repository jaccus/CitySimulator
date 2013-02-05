using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace City
{
    public class MapsQueryEngine
    {
        public static HashSet<PoiObject> Query(double latitude, double longitude, int radius, 
            IEnumerable<string> poiTypes)
        {
            var hashPois = new HashSet<PoiObject>();

            foreach (var poiType in poiTypes)
            {
                var url = string.Format("https://maps.googleapis.com/maps/api/place/search/xml?"
                                        + "location=" + CommonUtilities.DecimalDelimiterToDot(latitude)
                                                      + ","
                                                      + CommonUtilities.DecimalDelimiterToDot(longitude)
                                        + "&radius=" + radius
                                        + "&types=" + poiType
                                        + "&sensor=false"
                                        + "&key=" + Properties.Settings.Default.google_maps_key
                                        );

                var tmpPOIXml = Path.GetTempFileName();

                ReadResponseToFile(url, tmpPOIXml);

                var pois = PoisXmlToObjects(tmpPOIXml);

                File.Delete(tmpPOIXml);

                foreach (var poi in pois)
                    hashPois.Add(poi);
            }
            return hashPois;
        }

        private static IEnumerable<PoiObject> PoisXmlToObjects(string tmpPOIXml)
        {
            var document = XDocument.Load(tmpPOIXml);
            var pois = from c in document.Descendants("result")
                          select new PoiObject
                                     {
                                         // ReSharper disable PossibleNullReferenceException
                                         Vicinity = c.Element("vicinity").Value,
                                         
                                         Name = c.Element("name").Value,
                                         
                                         Latitude = double.Parse(c.Element("geometry")
                                         .Element("location").Element("lat").Value, 
                                         CultureInfo.InvariantCulture),
                                         
                                         Longitude = double.Parse(c.Element("geometry")
                                         .Element("location").Element("lng").Value, 
                                         CultureInfo.InvariantCulture),

                                         Types = c.Elements("type").Select(v => v.Value).ToList()
                                         // ReSharper restore PossibleNullReferenceException
                                     };
            return pois;
        }

        private static void ReadResponseToFile(string url, string tmpPOIXml)
        {
            Logger.LogInfo("Querying Google Maps API: {0}", url);

            var request = WebRequest.Create(url);

            var webResponse = request.GetResponse();
            if (webResponse == null) return;
            var stream = webResponse.GetResponseStream();


            if (stream == null) return;

            var streamReader = new StreamReader(stream);

            var sLine = "";
            var writer = new StreamWriter(tmpPOIXml);
            while (sLine != null)
            {
                sLine = streamReader.ReadLine();

                if (sLine == null) continue;

                writer.Write(sLine);
            }
            writer.Close();
        }

        public static string FormatXml(string inputXml)
        {
            var document = new XmlDocument();
            document.Load(new StringReader(inputXml));

            var builder = new StringBuilder();
            using (var writer = new XmlTextWriter(new StringWriter(builder)))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
            }

            return builder.ToString();
        }
    }
}