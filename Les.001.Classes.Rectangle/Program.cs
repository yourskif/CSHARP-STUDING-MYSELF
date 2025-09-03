using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public class Rectangle
    {
        private double side1, side2;

        public Rectangle(double side1, double side2)
        {
            this.side1 = side1;
            this.side2 = side2;
        }

        public double AreaCalculator()
        {
            return side1 * side2;
        }

        public double PerimeterCalculator()
        {
            return 2 * (side1 + side2);
        }

        public double Area
        {
            get { return AreaCalculator(); }
        }

        public double Perimeter
        {
            get { return PerimeterCalculator(); }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введіть довжину першої сторони: ");
            double side1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Введіть довжину другої сторони: ");
            double side2 = Convert.ToDouble(Console.ReadLine());

            Rectangle rect = new Rectangle(side1, side2);

            Console.WriteLine($"Периметр: {rect.Perimeter}");
            Console.WriteLine($"Площа: {rect.Area}");
        }
    }
}
