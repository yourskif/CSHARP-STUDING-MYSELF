namespace APP.DIWeekMonth.Services
{
    public class MonthsService : IStringProvider
    {
        public string[] GetValues()
        {
            return new[]
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };
        }
    }
}
