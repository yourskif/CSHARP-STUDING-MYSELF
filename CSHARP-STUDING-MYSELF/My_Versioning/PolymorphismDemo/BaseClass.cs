using System.Reflection;
using System;

namespace PolymorphismDemo
{
    public class BaseClass : IPrinter
    {
        // Шаблон NVI — метод TemplateMethod не virtual
        public void TemplateMethod()
        {
            PreAction();
            DoWork(); // virtual
            PostAction();
        }

        protected virtual void DoWork()
        {
            Console.WriteLine("Base DoWork");
        }

        private void PreAction()
        {
            Console.WriteLine("Base PreAction");
        }

        private void PostAction()
        {
            Console.WriteLine("Base PostAction");
        }

        public virtual void VirtualMethod()
        {
            Console.WriteLine("Base VirtualMethod");
        }

        public void HiddenMethod()
        {
            Console.WriteLine("Base HiddenMethod");
        }

        public virtual void Print()
        {
            Console.WriteLine("Base Print");
        }
    }
}
