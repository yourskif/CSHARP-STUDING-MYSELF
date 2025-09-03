using System;
using System.Collections.Generic;

class Animal
{
    public void Speak()
    {
        Console.WriteLine("Animal speaks.");
    }
}

class Dog : Animal
{
    public new void Speak()
    {
        Console.WriteLine("Woof!");
    }
}

class Program
{
    // Контраваріантний метод
    public static void HandleAnimal(Animal animal)
    {
        animal.Speak(); // Виводить відповідь для конкретної тварини (може бути Dog або Animal)
    }

    public static void Main()
    {
        // Створюємо список тварин (IEnumerable<Animal>)
        List<Dog> dogs = new List<Dog> { new Dog(), new Dog() };

        // І тут використовуємо контраваріантність:
        // Створюємо делегат, який приймає Animal, але ми можемо передати метод, який працює з Dog
        Action<Animal> animalAction = HandleAnimal;

        // Передаємо до делегата тип Dog, хоча він очікує Animal
        // Завдяки контраваріантності, ми можемо передавати Dog в метод, який приймає Animal
        foreach (var dog in dogs)
        {
            animalAction(dog);  // Виведе "Woof!" для кожного собаки
        }
    }
}
