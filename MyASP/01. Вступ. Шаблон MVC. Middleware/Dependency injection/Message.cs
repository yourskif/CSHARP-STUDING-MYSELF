namespace Dependency_injection
{
    public class Message
    {
        //Logger logger = new Logger();
        //public string Text { get; set; } = "";
        //public void Print() => logger.Log(Text);

        ILogger logger;
        public string Text { get; set; } = "";
        public Message(ILogger logger)
        {
            this.logger = logger;
        }
        public void Print() => logger.Log(Text);
    }
}
