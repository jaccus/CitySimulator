using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace City.Test
{
    [TestClass]
    public class DBUtilitiesTest
    {
        const string SchemaFile = @"D:\programming\visual studio 10\projects\CitySilverlight\bin\Debug\CitySchema.sqlce";
        const string DBFile = @"D:\programming\visual studio 10\projects\CitySilverlight\bin\Debug\CityCompact.sdf";

        [TestMethod]
        public void TestScriptGeneration()
        {
            using(var ctx = new CityContainer())
            {
                var databaseScript = ctx.CreateDatabaseScript();

                File.WriteAllText(@"D:\db.sql", databaseScript);
            }
        }

        [TestMethod]
        public void DeleteDatabaseTest()
        {
            using (var ctx = new CityContainer())
            {
                ctx.DeleteDatabase();
            }
        }

        [TestMethod]
        public void CreateIfNotExist()
        {
            using (var ctx = new CityContainer())
            {
                if(!ctx.DatabaseExists())
                    ctx.CreateDatabase();
            }
        }

        [TestMethod]
        public void CheckTableExists()
        {
            if (!DBUtilities.TableExistsSqlCE("Demands"))
            {
                DBUtilities.ImportSchemaCEIfNeeded();
            }
        }

        [TestMethod]
        public void ListTablesTest()
        {
            var tables = DBUtilities.ListTablesCE();
        }
    }
}
