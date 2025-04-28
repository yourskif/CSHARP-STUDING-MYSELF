using System;
using System.Collections;

namespace WeekEnumerable
{
    class Week : IEnumerable
    {
        private string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        public IEnumerator GetEnumerator()
        {
            return new WeekEnumerator(days);
        }

        /* Альтернативний варіант з використанням yield:
        public IEnumerator GetEnumerator()
        {
            foreach (var day in days)
            {
                yield return day;
            }
        }
        */
    }

    class WeekEnumerator : IEnumerator
    {
        private string[] _days;
        private int position = -1;

        public WeekEnumerator(string[] days)
        {
            _days = days;
        }

        public object Current
        {
            get
            {
                if (position < 0 || position >= _days.Length)
                    throw new InvalidOperationException();
                return _days[position];
            }
        }

        public bool MoveNext()
        {
            if (position < _days.Length - 1)
            {
                position++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            position = -1;
        }
    }

    class Program
    {
        static void Main()
        {
            Week week = new Week();
            foreach (var day in week)
            {
                Console.WriteLine(day);
            }
        }
    }
}
