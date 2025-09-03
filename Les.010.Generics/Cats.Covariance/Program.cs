using System;
using System.Collections.Generic;

class Animal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("Some animal sound...");
    }
}

class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Meow!");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Створюємо список котів (IEnumerable<Cat>)
        List<Cat> cats = new List<Cat>
        {
            new Cat(),
            new Cat()
        };

        // Тепер ми можемо присвоїти його змінній типу IEnumerable<Animal> (через коваріантність)
        IEnumerable<Animal> animals = cats;

        // Перебираємо список тварин (але насправді це коти)
        foreach (var animal in animals)
        {
            animal.MakeSound();  // Виведе "Meow!" для кожного кота
        }
    }
}
