namespace City.Test
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CommonUtilitiesTest
    {
        private const string FirstStatement = @"ALTER TABLE [CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];";

        private const string SecondStatement = @"ALTER TABLE [Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];";

        private const string ThirdStatement = @"ALTER TABLE [Transactions] DROP CONSTRAINT [FK_PoiTransaction];";

        private const string ThreeStatementsBlockString =
            "\r\n    ALTER TABLE [CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];\r\nGO\r\n    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];\r\nGO\r\n    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_PoiTransaction];";

        [TestMethod]
        public void ReadSqlCeCommandsFromString_GivenMultipleCommands_ExtractsThemToEnumerable()
        {
            var actualCommands = CommonUtilities.ReadSqlCeCommandsFromString(ThreeStatementsBlockString).ToList();

            Assert.Equals(3, actualCommands.Count);
            Assert.Equals(FirstStatement, actualCommands[0]);
            Assert.Equals(SecondStatement, actualCommands[1]);
            Assert.Equals(ThirdStatement, actualCommands[2]);
        }
    }
}