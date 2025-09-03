using System;

namespace PolymorphismDemo
{
    public class DerivedClass : BaseClass
    {
        protected override void DoWork()
        {
            Console.WriteLine("Derived DoWork");
        }

        public override void VirtualMethod()
        {
            Console.WriteLine("Derived VirtualMethod");
        }

        public new void HiddenMethod()
        {
            Console.WriteLine("Derived HiddenMethod");
        }

        public override void Print()
        {
            Console.WriteLine("Derived Print");
        }
    }
}
