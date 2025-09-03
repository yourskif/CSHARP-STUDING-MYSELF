namespace Получение_зависимостей
{
    public class ShortTimeService : ITimeService
    {
        public string GetTime() => DateTime.Now.ToShortTimeString();
    }
}
