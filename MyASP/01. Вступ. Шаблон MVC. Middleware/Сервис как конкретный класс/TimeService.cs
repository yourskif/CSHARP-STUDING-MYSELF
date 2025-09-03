namespace Сервис_как_конкретный_класс
{
    public class TimeService
    {
        public string GetTime() => DateTime.Now.ToShortTimeString();
    }
}
