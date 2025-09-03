/*
 Створи програму, яка моделює банківський рахунок.

    Метод Withdraw(decimal amount) знімає гроші з рахунку.

    Якщо після зняття коштів баланс стає меншим за 100, має бути згенерована подія LowBalance.

    Подія повинна передавати новий баланс через аргументи.

    Обробник події виводить попередження з балансом.

Вимоги:

    Створи подію LowBalance типу EventHandler<BalanceEventArgs>.

    Клас BalanceEventArgs повинен мати властивість decimal Balance.

    Метод Withdraw приймає параметр від користувача (з Main) і викликає подію, якщо треба.


* 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankAccount
{
    public class MyAccount
    {
        public event EventHandler<BalanceEventArgs> LowBalance;

        public int Amount {  get; set; }

        public decimal Withdraw(decimal amount)
        {
            Amount -= (int)amount;
            if (Amount < 100)
            {
                LowBalance?.Invoke(this, new BalanceEventArgs(Amount));
            }
            return Amount;
        }
    }

    public class BalanceEventArgs : EventArgs
    {
        public decimal Balance { get; set; }
        public BalanceEventArgs(decimal balance)
        {
            Balance = balance;
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            var account = new MyAccount();
            account.LowBalance += Account_LowBalance;
            account.Amount = 150;
            account.LowBalance += Account_LowBalance;
            account.Withdraw(60);  // баланс = 90 => подія буде


        }
        static void Account_LowBalance(object sender, BalanceEventArgs e)
        {
            Console.WriteLine($"Увага! Баланс низький: {e.Balance} грн.");
        }

    }
}
