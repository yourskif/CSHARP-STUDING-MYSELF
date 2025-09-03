namespace Life_cycle
{
    public class CounterService
    {
        public ICounter Counter { get; }
        public CounterService(ICounter counter)
        {
            Counter = counter;
        }
    }
}
