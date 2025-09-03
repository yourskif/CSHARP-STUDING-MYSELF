using System;
using System.Collections;

class MyCollection : IEnumerable
{
    private int[] data = { 10, 20, 30 };

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < data.Length; i++)
        {
            yield return data[i];
        }
    }
}

class Program
{
    static void Main()
    {
        MyCollection collection = new MyCollection();

        foreach (int item in collection)
        {
            Console.WriteLine(item);
        }
    }
}
