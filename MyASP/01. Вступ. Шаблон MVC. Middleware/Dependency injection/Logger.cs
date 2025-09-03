namespace Dependency_injection
{
    public class Logger : ILogger
    {
        public void Log(string message) => Console.WriteLine(message);
    }

}
