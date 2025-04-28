
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectInvoice
{
    public class Invoice
    {
        public int Account { get; }
        public string Customer { get; }
        public string Provider { get; }
        private string article;
        private int quantity;
        private decimal price;

        public Invoice(int account, string customer, string provider, string article, int quantity, decimal price)
        {
            Account = account;
            Customer = customer;
            Provider = provider;
            this.article = article;
            this.quantity = quantity;
            this.price = price;
        }

        public decimal CalculateTotalCost(bool includeVAT)
        {
            const decimal VAT_RATE = 0.2m;
            decimal totalCost = price * quantity;
            return includeVAT ? totalCost * (1 + VAT_RATE) : totalCost;
        }
    }

    class Program
    {
        static void Main()
        {
            Invoice invoice = new Invoice(12345, "John Doe", "TechCorp", "Laptop", 2, 1200m);
            Console.WriteLine($"Account: {invoice.Account}, Customer: {invoice.Customer}, Provider: {invoice.Provider}");
            Console.WriteLine("Enter VAT option (yes/no):");
            string input = Console.ReadLine();
            bool includeVAT = input?.Trim().ToLower() == "yes";
            decimal totalCost = invoice.CalculateTotalCost(includeVAT);
            Console.WriteLine($"Total cost {(includeVAT ? "with" : "without")} VAT: {totalCost:C}");
        }
    }
}
