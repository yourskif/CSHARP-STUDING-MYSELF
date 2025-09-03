namespace APP.DIWeekMonth.Services
{
    public class DaysOfWeekService : IStringProvider
    {
        public string[] GetValues()
        {
            return new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        }
    }
}
