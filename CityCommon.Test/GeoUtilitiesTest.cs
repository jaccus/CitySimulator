using System;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace City.Test
{
    [TestClass]
    public class GeoUtilitiesTest
    {
        public TestContext TestContext { get; set; }

        [DeploymentItem("CityCommon.Test\\resources\\PointInPolygonData.xml"), TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\resources\\PointInPolygonData.xml", "Row", DataAccessMethod.Sequential)]
        public void PointInPolygonTest()
        {
            var polygonPoints = new LocationCollection
                                    {
                                        new Location(0,0),
                                        new Location(0,10),
                                        new Location(10,10),
                                        new Location(10,0)
                                    };

            var expectedResult = bool.Parse(TestContext.DataRow["Result"].ToString());
            
            var latitude = double.Parse(TestContext.DataRow["lat"].ToString());
            var longitude = double.Parse(TestContext.DataRow["lng"].ToString());
            var actualResult = GeoUtilities.IsPointInPolygon(polygonPoints, new Location(latitude, longitude));

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void PolygonBorderTest()
        {
            var polygonPoints = GetPentagonPoints();

            var actualBorder = GeoUtilities.PolygonBorder(polygonPoints);

            var expectedBorder = new GeoRectangle(1, 8, 1, 6);

            Assert.AreEqual(expectedBorder, actualBorder);

        }

        private static LocationCollection GetPentagonPoints()
        {
            return new LocationCollection
                       {
                           new Location(1, 3),
                           new Location(2, 5),
                           new Location(5, 6),
                           new Location(8, 4),
                           new Location(4, 1)
                       };
        }

        [TestMethod]
        public void NullValueIfRandomPointWithinPolygonWithTwoVertices()
        {
            var twoVertices = new LocationCollection
                               {
                                   new Location(1, 2),
                                   new Location(3, 4)
                               };

            var actual = GeoUtilities.RandomLocationWithinPolygon(twoVertices);
            
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void CalculateBingRouteTest()
        {
            //GeoUtilities.CalculateBingRoute();
        }

        [TestMethod]
        public void DistanceTest()
        {
            var distance = GeoUtilities.DirectDistance(new Location(5, 70), new Location(12, 40));
            Assert.AreEqual(3387, Math.Round(distance));
        }
    }
}