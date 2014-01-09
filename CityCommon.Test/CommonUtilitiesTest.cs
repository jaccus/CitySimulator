namespace City.Test
{
    using System.Linq;

    using Xunit;

    public class CommonUtilitiesTest
    {
        private const string FirstStatement = @"    ALTER TABLE [CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];";

        private const string SecondStatement = @"    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];";

        private const string ThirdStatement = @"    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_PoiTransaction];";

        private const string ThreeStatementsBlockString =
            "\n    ALTER TABLE [CreditCards] DROP CONSTRAINT [FK_CreditCardPerson];\nGO\n    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_CreditCardTransactions];\nGO\n    ALTER TABLE [Transactions] DROP CONSTRAINT [FK_PoiTransaction];";

        [Fact]
        public void ReadSqlCeCommandsFromString_GivenMultipleCommands_ExtractsThemToEnumerable()
        {
            var actualCommands = CommonUtilities.ReadSqlCeCommandsFromString(ThreeStatementsBlockString.Split('\n')).ToList();

            Assert.Equal(3, actualCommands.Count);
            Assert.Equal(FirstStatement, actualCommands[0]);
            Assert.Equal(SecondStatement, actualCommands[1]);
            Assert.Equal(ThirdStatement, actualCommands[2]);
        }
    }
}