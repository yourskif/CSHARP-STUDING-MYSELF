using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3.FindFibonacciNumberRecursive
{
    internal class Program
    {
        public static int FibonacciRecursive(int n)
        {
            if (n <= 1)
                return n;
           return FibonacciRecursive(n - 1) + FibonacciRecursive(n - 2);
        }


        static void Main(string[] args)
        {
            int n = 5;
            int result = FibonacciRecursive(n);
            Console.WriteLine($"Fibonacci({n}) = {result}"); 
        } 


    }
}
