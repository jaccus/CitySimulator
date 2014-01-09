using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace City.Test
{
    using Xunit;

    public class DBUtilitiesTest
    {
        const string SchemaFile = @"D:\programming\visual studio 10\projects\CitySilverlight\bin\Debug\CitySchema.sqlce";
        const string DBFile = @"D:\programming\visual studio 10\projects\CitySilverlight\bin\Debug\CityCompact.sdf";

        [Fact]
        public void TestScriptGeneration()
        {
            using(var ctx = new CityContainer())
            {
                var databaseScript = ctx.CreateDatabaseScript();

                File.WriteAllText(@"D:\db.sql", databaseScript);
            }
        }

        [Fact]
        public void DeleteDatabaseTest()
        {
            using (var ctx = new CityContainer())
            {
                ctx.DeleteDatabase();
            }
        }

        [Fact]
        public void CreateIfNotExist()
        {
            using (var ctx = new CityContainer())
            {
                if(!ctx.DatabaseExists())
                    ctx.CreateDatabase();
            }
        }

        [Fact]
        public void CheckTableExists()
        {
            if (!DBUtilities.TableExistsSqlCE("Demands"))
            {
                DBUtilities.ImportSchemaCEIfNeeded();
            }
        }

        [Fact]
        public void ListTablesTest()
        {
            var tables = DBUtilities.ListTablesCE();
        }
    }
}
