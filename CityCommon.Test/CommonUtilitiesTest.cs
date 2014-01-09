using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace City.Test
{
    [TestClass]
    public class CommonUtilitiesTest
    {
        [TestMethod]
        public void ReadCommandsTest()
        {
            var text = @"D:\test.sqlce";

            var commands = CommonUtilities.ReadSqlCeCommandsFromString(File.ReadAllText(text));

        }
    }
}
