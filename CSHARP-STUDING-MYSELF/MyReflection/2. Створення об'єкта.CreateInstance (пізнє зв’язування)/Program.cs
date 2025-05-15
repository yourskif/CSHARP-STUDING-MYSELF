using System;

class Animal
{
    public string Species { get; set; } = "Cat";
    public void Speak() => Console.WriteLine($"I am a {Species}");
}

class Program
{
    static void Main()
    {
        Type type = typeof(Animal);
        object obj = Activator.CreateInstance(type);

        var method = type.GetMethod("Speak");
        method.Invoke(obj, null);
    }
}
