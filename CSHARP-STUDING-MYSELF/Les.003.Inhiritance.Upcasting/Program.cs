using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 1. Базовий клас
class Animal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("Some sound...");
    }
}

// 2. Похідний клас Dog
class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Woof! Woof!");
    }
}

// 3. Похідний клас Cat (sealed)
sealed class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Meow!");
    }
}

class Program
{
    static void Main()
    {
        // 4. Upcasting (підняття до базового типу)
        Animal myDog = new Dog();
        myDog.MakeSound(); // Woof! Woof!

        // 5. Downcasting (приведення до похідного типу)
        Dog realDog = (Dog)myDog;
        realDog.MakeSound(); // Woof! Woof!

        // 6. Спроба успадкування від sealed-класу (Це викличе помилку)
        // class Tiger : Cat {} // Помилка компіляції
    }
}
