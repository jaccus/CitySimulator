namespace City.Test
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CommonUtilitiesTest
    {
        private const string FirstStatement = @"    ALTER TABLE [CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];";

        private const string SecondStatement = @"    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];";

        private const string ThirdStatement = @"    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_PoiTransaction];";

        private const string ThreeStatementsBlockString =
            "\n    ALTER TABLE [CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];\nGO\n    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];\nGO\n    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_PoiTransaction];";

        [TestMethod]
        public void ReadSqlCeCommandsFromString_GivenMultipleCommands_ExtractsThemToEnumerable()
        {
            var actualCommands = CommonUtilities.ReadSqlCeCommandsFromString(ThreeStatementsBlockString.Split('\n')).ToList();

            Assert.AreEqual(3, actualCommands.Count);
            Assert.AreEqual(FirstStatement, actualCommands[0]);
            Assert.AreEqual(SecondStatement, actualCommands[1]);
            Assert.AreEqual(ThirdStatement, actualCommands[2]);
        }
    }
}