using System;

namespace SinglyLinkedListExample
{
    public class Node<T>
    {
        public T Value { get; set; }
        public Node<T> Next { get; set; }

        public Node(T value)
        {
            Value = value;
            Next = null;
        }
    }

    public class SinglyLinkedList<T>
    {
        private Node<T> head;

        public void Add(T value)
        {
            Node<T> newNode = new Node<T>(value);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                Node<T> current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
        }

        public void Print()
        {
            Node<T> current = head;
            while (current != null)
            {
                Console.Write(current.Value + " -> ");
                current = current.Next;
            }
            Console.WriteLine("null");
        }
    }

    class Program
    {
        static void Main()
        {
            SinglyLinkedList<int> list = new SinglyLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Print(); // Виведе: 1 -> 2 -> 3 -> null
        }
    }
}
