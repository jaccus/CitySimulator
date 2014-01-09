using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Maps.MapControl.WPF;

namespace City
{
    public static class DBUtilities
    {
        public delegate void NextDemandPool(string configurationName);
        

        public static void InsertStaticDataIfNeeded()
        {
            if(StaticDataExists())
            {
                Logger.LogInfo("Static data already exists. Skipping static data generation.");
                return;
            }

            var poiList = File.ReadLines(Properties.Settings.Default.poi_file);
            var currencies = File.ReadLines(Properties.Settings.Default.currencies_file);
            var banks = File.ReadLines(Properties.Settings.Default.banks_file);
            var simulationMethods = SymulationType.List();

            using(var ctx = new CityContainer())
            {
                foreach (var poi in poiList)
                    ctx.PoiTypes.AddObject(new PoiType {Code = poi});

                foreach(var currency in currencies)
                    ctx.Currencies.AddObject(new Currency{Code = currency});

                foreach (var bank in banks)
                    ctx.Banks.AddObject(new Bank {Name = bank});

                foreach (var simulationMethod in simulationMethods)
                    ctx.SelectionMethods.AddObject(new SelectionMethod {Name = simulationMethod});

                ctx.SaveChanges();
            }

            InsertPeopleTemplates();
        }

        public static bool StaticDataExists()
        {
            using (var ctx = new CityContainer())
            {
                return ctx.PoiTypes.Count() != 0;
            }
        }

        public static bool TableExistsSqlCE(string tableName)
        {
            var sqlCeTableExists = @"SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='"
                                            + tableName + "'";

            using (var ctx = new CityContainer())
            {
                var exists = ctx.ExecuteStoreQuery<int>(sqlCeTableExists).SingleOrDefault();

                return exists == 1 ? true : false;
            }
        }

        public static bool SaveConfiguration(ConfigurationData data)
        {
            using (var ctx = new CityContainer())
            {
                var configurationExists = ctx.Configurations.Any(c => c.Name == data.ConfigurationName);
                if (configurationExists) return false;

                var poiTypes = ctx.PoiTypes.Where(t => data.PoiTypes.Contains(t.Code));

                var newConfiguration = new Configuration
                                           {
                                               Name = data.ConfigurationName,
                                               SelectionMethod = ctx.SelectionMethods.Single(m => m.Name == data.SimulationMethodName),
                                               SimulationStartDate = data.SimulationStartDate,
                                               SimulationEndDate = data.SimulationEndDate,
                                               PersonDemandsPerDay = data.DemandsPerPersonPerDay,
                                               CreditCardBalanceMin = data.CreditCardBalanceMin,
                                               CreditCardBalanceMax = data.CreditCardBalanceMax,
                                               CreditCardLimitMin = data.CreditCardLimitMin,
                                               CreditCardLimitMax = data.CreditCardLimitMax,
                                               TransactionValueMin = data.TransactionValueMin,
                                               TransactionValueMax = data.TransactionValueMax
                                           };
                
                // Assign POI circle areas
                foreach (var circleAreaObject in data.CircleAreas)
                {
                    newConfiguration.CircleAreas.Add(new CircleArea
                                                         {
                                                             Configuration = newConfiguration,
                                                             Lat = circleAreaObject.Pushpin.Location.Latitude,
                                                             Lng = circleAreaObject.Pushpin.Location.Longitude,
                                                             Range = circleAreaObject.Range
                                                         });
                }

                // Assign POI types for configuration
                foreach (var poiType in poiTypes)
                    newConfiguration.PoiTypes.Add(poiType);

                // Assign Districts for configuration
                foreach (var polygon in data.Polygons)
                {
                    var district = new District
                                       {
                                           Configuration = newConfiguration,
                                           Population = polygon.Population,
                                           Name = polygon.DistrictName
                                       };

                    foreach (var location in polygon.Locations)
                    {
                        district.MapPoints.Add(new MapPoint
                                                   {
                                                       Lat = location.Latitude,
                                                       Lng = location.Longitude
                                                   });
                    }
                }

                ctx.Configurations.AddObject(newConfiguration);
                ctx.SaveChanges();

                return true;
            }
        }

        public static IEnumerable<string> ListConfigurations()
        {
            var cfgNames = new List<string>();

            using (var ctx = new CityContainer())
                cfgNames.AddRange(ctx.Configurations.Select(configuration => configuration.Name));

            return cfgNames;
        }

        public static bool RemoveConfiguration(string cfgName)
        {
            try
            {
                using (var ctx = new CityContainer())
                {
                    ctx.ContextOptions.LazyLoadingEnabled = false;

                    Logger.LogInfo("Removing configuration '{0}'", cfgName);
                    var configuration = ctx.Configurations
                        //.Include("Districts.Addresses.Residents.Demands.PoiTypes")
                        //.Include("Districts.Addresses.Residents.CreditCard")
                        .Include("Pois") //poi
                        .Include("CircleAreas")
                        .Include("Pois.Type")
                        .Include("PoiTypes")
                        .Include("Pois.MapPoint")
                        .Include("Districts.MapPoints")
                        .Include("Districts.Addresses.MapPoint")
                        .Single(s => s.Name == cfgName);

                    foreach (var poi in configuration.Pois)
                    {
                        poi.Type.Clear();
                        var mapPoint = poi.MapPoint;
                        ctx.Attach(mapPoint);
                        ctx.DeleteObject(mapPoint);
                    }

                    configuration.PoiTypes.Clear();

                    foreach (var mapPoint in
                        configuration.Districts.SelectMany(district => district.Addresses.Select(
                            address => address.MapPoint)))
                    {
                        ctx.Attach(mapPoint);
                        ctx.DeleteObject(mapPoint);
                    }

                    ctx.Configurations.DeleteObject(configuration);

                    ctx.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IEnumerable<string> ListSimulationTypes()
        {
            var typeNames = new List<string>();
            
            using(var ctx = new CityContainer())
            {
                typeNames.AddRange(ctx.SelectionMethods.Select(m => m.Name));
            }
            
            return typeNames;
        }

        public static ConfigurationData LoadConfiguration(string configurationName)
        {
            var data = new ConfigurationData();

            using(var ctx = new CityContainer())
            {
                var configuration = ctx.Configurations.Single(c => c.Name == configurationName);

                data.ConfigurationName = configurationName;

                // Load POI circle areas

                var circleAreaObjects = new List<CircleAreaObject>();

// ReSharper disable LoopCanBeConvertedToQuery
                foreach (var circleArea in configuration.CircleAreas)
// ReSharper restore LoopCanBeConvertedToQuery
                    circleAreaObjects.Add(new CircleAreaObject(circleArea.Lat, circleArea.Lng, circleArea.Range));

                data.CircleAreas = circleAreaObjects;

                // Load POI types

                data.PoiTypes = configuration.PoiTypes.Select(poiType => poiType.Code).ToList();

                var polygons = new List<MapDistrict>();
                foreach (var district in configuration.Districts)
                {
                    var newPolygon = new MapDistrict {DistrictName = district.Name, Population = district.Population, Locations = new LocationCollection()};

                    foreach (var mapPoint in district.MapPoints)
                        newPolygon.Locations.Add(new Location(mapPoint.Lat, mapPoint.Lng));

                    polygons.Add(newPolygon);
                }

                data.Polygons = polygons;

                // simulation settings
                data.SimulationMethodName = configuration.SelectionMethod.Name;
                data.DemandsPerPersonPerDay = configuration.PersonDemandsPerDay;
                data.SimulationStartDate = configuration.SimulationStartDate;
                data.SimulationEndDate = configuration.SimulationEndDate;

                data.CreditCardBalanceMin = configuration.CreditCardBalanceMin;
                data.CreditCardBalanceMax = configuration.CreditCardBalanceMax;
                data.CreditCardLimitMin = configuration.CreditCardLimitMin;
                data.CreditCardLimitMax = configuration.CreditCardLimitMax;
                data.TransactionValueMin = configuration.TransactionValueMin;
                data.TransactionValueMax = configuration.TransactionValueMax;
            }
            return data;
        }

        public static void InsertPeopleTemplates()
        {
            var maleEntries = File.ReadLines(Properties.Settings.Default.male_file);
            var femaleEntries = File.ReadLines(Properties.Settings.Default.female_file);

            using(var ctx = new CityContainer())
            {
                // Add male templates
                foreach (var split in maleEntries.Select(male => male.Split(';')))
                    ctx.PersonTemplates.AddObject(
                        new PersonTemplate { FirstName = split[0], LastName = split[1], IsMale = true });

                // Add female templates
                foreach (var split in femaleEntries.Select(female => female.Split(';')))
                    ctx.PersonTemplates.AddObject(
                        new PersonTemplate { FirstName = split[0], LastName = split[1], IsMale = false });

                ctx.SaveChanges();
            }
        }

        public static void GeneratePOI(string cfgName, IEnumerable<PoiObject> poiObjects)
        {
            using (var ctx = new CityContainer())
            {
                var configuration = ctx.Configurations.Single(c => c.Name == cfgName);
                
                var lastPoiType = string.Empty;
                try
                {
                    foreach (var poiObject in poiObjects)
                    {
                        var poi = new Poi
                                      {
                                          Configuration = configuration,
                                          Vicinity = poiObject.Vicinity,
                                          Name = poiObject.Name,
                                          MapPoint = new MapPoint
                                                         {
                                                             Lat = poiObject.Latitude,
                                                             Lng = poiObject.Longitude
                                                         }
                                      };

                        foreach (var type in poiObject.Types)
                        {
                            lastPoiType = type;

                            string poiType = lastPoiType;
                            poi.Type.Add(ctx.PoiTypes.Single(t => t.Code == poiType));

                        }
                        ctx.Pois.AddObject(poi);
                    }
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.LogError("Unsupported POI type '{0}'. Please consider adding this "
                        + "type to poi types configuration file.\n{2}", lastPoiType, e.Message);
                }
            }
        }

        public static void GeneratePOI(string cfgName, string poiXml)
        {
            using (var ctx = new CityContainer())
            {
                var configuration = ctx.Configurations.Single(c => c.Name == cfgName);

                var document = XDocument.Load(poiXml);
                var results = from c in document.Descendants("result")
                              select new {
// ReSharper disable PossibleNullReferenceException
                                             Vicinity = c.Element("vicinity").Value,
                                             Name = c.Element("name").Value,
                                             Lat = c.Element("geometry").Element("location").Element("lat").Value,
                                             Lng = c.Element("geometry").Element("location").Element("lng").Value,
                                             Types = c.Elements("type").Select(v => v.Value)
// ReSharper restore PossibleNullReferenceException
                                         };

                foreach (var result in results)
                {
                    var poi = new Poi
                                  {
                                      Configuration = configuration,
                                      Vicinity = result.Vicinity,
                                      Name = result.Name,
                                      MapPoint = new MapPoint
                                      {
                                          Lat = CommonUtilities.ParseDouble(result.Lat),
                                          Lng = CommonUtilities.ParseDouble(result.Lng)
                                      }
                                  };
                    var lastPoiType = string.Empty;
                    try
                    {
                        foreach (var type in result.Types)
                        {
                            lastPoiType = type;

                            var poiType = lastPoiType;
                            poi.Type.Add(ctx.PoiTypes.Single(t => t.Code == poiType));

                        }
                        ctx.Pois.AddObject(poi);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Unsupported POI type '{0}'. Please consider adding this type to poi types configuration file.\n{2}", lastPoiType, e.Message);
                    }
                }

                ctx.SaveChanges();
            }
            File.Delete(poiXml);
        }

        public static string DbPath
        {
            get
            {
                return string.Format("{0}/{1}",
                AppDomain.CurrentDomain.GetData("DataDirectory"),
                Properties.Settings.Default.ce_dbname);
            }
        }

        private static string CEConnectionString
        {
            get { return string.Format("data source='{0}'", DbPath); }
        }

        public static void ImportSchemaCEIfNeeded()
        {
            if (TableExistsSqlCE("Demands"))
            {
                Logger.LogInfo("Schema already exists. Skipping schema import.");
                return;
            }

            var connectionString = CEConnectionString;
            Logger.LogInfo("Importing SQL CE schema. Connection string: {0}", connectionString);

            using (var connection = new SqlCeConnection(connectionString))
            {
                var ceSchema = Path.GetFullPath(Properties.Settings.Default.ce_schema);
                string text = File.ReadAllText(ceSchema);
                var commands = CommonUtilities.ReadSqlCeCommandsFromString(text, CommonUtilities.ExtractLinesToActuallyReadFrom(text));

                var cmd = new SqlCeCommand {Connection = connection};
                connection.Open();
                foreach (var command in commands)
                {
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
            Logger.LogInfo("SQL CE schema imported.");
        }

        public static IEnumerable<string> ListTablesCE()
        {
            var tableNames = new List<string>();

            using (var connection = new SqlCeConnection(CEConnectionString))
            {
                var cmd = new SqlCeCommand { Connection = connection };
                connection.Open();
                cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tableNames.Add(reader["TABLE_NAME"] as string);
                }
                reader.Close();
            }
            return tableNames;
        }

        public static void CreateDatabaseIfNeeded()
        {
            if (File.Exists(DbPath))
            {
                Logger.LogInfo("Database exists at '{0}'. Skipping db creation.", DbPath);
                return;
            }

            Logger.LogInfo("Creating database at '{0}'.", DbPath);

            var startInfo = new ProcessStartInfo
                                {
                                    FileName = "SqlCeCmd40.exe",
                                    WorkingDirectory = Directory.GetCurrentDirectory() + "\\tools",
                                    CreateNoWindow = true,
                                    UseShellExecute = true,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    Arguments = string.Format(
                                        "-d \"Data Source={0}\" -e \"create\"", DbPath),
                                };
            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
    }
}