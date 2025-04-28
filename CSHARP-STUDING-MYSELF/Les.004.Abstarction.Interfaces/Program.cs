using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAbstaraction
{
    abstract class Animal
    {
        public abstract void MakeSound();

        public void Sleep()
        {
            Console.WriteLine("Sleeping");
        }
    }

    interface IAnimalsActions
    {
        void Eat();
        void Move();
        void Play();
    }

    class Dog : Animal, IAnimalsActions
    {
        public override void MakeSound()
        {
            Console.WriteLine("Bark!");
        }

        public void Eat() // Додаємо public
        {
            Console.WriteLine("Eating dog food.");
        }

        public void Move() // Додаємо public
        {
            Console.WriteLine("Running...");
        }
        public void Play()
        {
            Console.WriteLine("Playing...in Dog");
        }

    }

    class Cat : Animal, IAnimalsActions
    {
        public override void MakeSound()
        {
            Console.WriteLine("MAU..MAU!");
        }

        public void Eat() // Додаємо public
        {
            Console.WriteLine("Eating dog food.");
        }

        public void Move() // Додаємо public
        {
            Console.WriteLine("Running...");
        }

        public void Play()
        {
            Console.WriteLine("Playing...in Cat ");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dog myDog = new Dog();
            myDog.MakeSound(); // Виклик абстрактного методу
            myDog.Sleep();     // Виклик звичайного методу
            myDog.Eat();       // Виклик методу інтерфейсу
            myDog.Move();      // Виправлено виклик методу (було "Movie")
            myDog.Play();

            Cat myCat = new Cat();
            myCat.MakeSound(); // Виклик абстрактного методу
            myCat.Sleep();     // Виклик звичайного методу
            myCat.Eat();       // Виклик методу інтерфейсу
            myCat.Move();      // Виправлено виклик методу (було "Movie")
            myCat.Play();

            Console.WriteLine("Hello, World!");
        }
    }
}
