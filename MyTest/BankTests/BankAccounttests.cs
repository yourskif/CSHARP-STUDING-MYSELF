using BankAccountNS;

namespace BankTests
{
    [TestClass]
    public sealed class BankAccounttests
    {
        [TestMethod]
        public void Debit_WithValidAmount_UpdatesBalance()
        {
            // Arrange
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act
            account.Debit(debitAmount);

            // Assert
            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
        }

        [TestMethod]
        public void Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange()
        {
            // Arrange
            double beginningBalance = 11.99;
            double debitAmount = 20.0;
            BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            // Act
            try
            {
                account.Debit(debitAmount);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                // Assert
                StringAssert.Contains(e.Message, BankAccount.DebitAmountExceedsBalanceMessage);
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }

        //[TestMethod]
        //public void Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange()
        //{
        //    // Arrange
        //    double beginningBalance = 11.99;
        //    double debitAmount = 20.0;
        //    BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        //    // Act
        //    try
        //    {
        //        account.Debit(debitAmount);
        //    }
        //    catch (System.ArgumentOutOfRangeException e)
        //    {
        //        // Assert
        //        StringAssert.Contains(e.Message, BankAccount.DebitAmountExceedsBalanceMessage);
        //    }
        //}

        //[TestMethod]
        //public void Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange()
        //{
        //    // Arrange
        //    double beginningBalance = 11.99;
        //    double debitAmount = 20.0;
        //    BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        //    // Act
        //    try
        //    {
        //        account.Debit(debitAmount);
        //    }
        //    catch (System.ArgumentOutOfRangeException e)
        //    {
        //        // Assert
        //        StringAssert.Contains(e.Message, BankAccount.DebitAmountExceedsBalanceMessage);
        //    }
        //}

    }
}
