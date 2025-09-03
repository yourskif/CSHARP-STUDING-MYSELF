/*
Yuriy Antonov copyright 2018-2020
*/

namespace ConsoleMenu
{
    public class Menu
    {
        private readonly Dictionary<ConsoleKey, MenuItem> items;

        public Menu()
        {
            this.items = new Dictionary<ConsoleKey, MenuItem>();
        }

        public Menu(ConsoleKey id, string caption, Action action)
        {
            this.items = new Dictionary<ConsoleKey, MenuItem>();
            this.items.Add(id, new MenuItem(caption, action));
        }

        public Menu((ConsoleKey id, string caption, Action action)[] array)
        {
            ArgumentNullException.ThrowIfNull(array);
            this.items = new Dictionary<ConsoleKey, MenuItem>();
            foreach (var elem in array)
            {
                if (!this.items.ContainsKey(elem.id))
                {
                    this.items.Add(elem.id, new MenuItem(elem.caption, elem.action));
                }
            }
        }

        public virtual void Run()
        {
            ConsoleKey resKey;
            bool updateItems = true;
            do
            {
                resKey = this.RunOnce(ref updateItems);
            }
            while (resKey != ConsoleKey.Escape);
        }

        public /*virtual*/ ConsoleKey RunOnce(ref bool updateItems)
        {
            ConsoleKeyInfo res;
            if (updateItems)
            {
                    foreach (var item in this.items)
                    {
                        Console.WriteLine($"<{item.Key}>:  {item.Value}");
                    }

                    Console.WriteLine("Or press <Esc> to return");
            }

            res = Console.ReadKey(true);
            if (this.items.ContainsKey(res.Key))
            {
                this.items[res.Key].Action();
                updateItems = true;
            }
            else
            {
                updateItems = false;
            }

            return res.Key;
        }

        public void RunOld()
        {
            ConsoleKeyInfo res;
            bool updateItems = true;
            do
            {
                if (updateItems)
                {
                    foreach (var item in this.items)
                    {
                        Console.WriteLine($"<{item.Key}>:  {item.Value}");
                    }

                    Console.WriteLine("Or press <Esc> to return");
                }

                res = Console.ReadKey(true);
                if (this.items.ContainsKey(res.Key))
                {
                    this.items[res.Key].Action();
                    updateItems = true;
                }
                else
                {
                    updateItems = false;
                }
            }
            while (res.Key != ConsoleKey.Escape);
        }
    }
}