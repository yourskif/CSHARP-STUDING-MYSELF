namespace Передача_зависимостей_в_конечные_точки
{
    // сервис
    public class TimeService
    {
        public string Time => DateTime.Now.ToLongTimeString();
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //Передача_зависимостей_в_конечные_точки
            //https://localhost:7145/time
            //var builder = WebApplication.CreateBuilder();

            //builder.Services.AddTransient<TimeService>();   // Добавляем сервис

            //var app = builder.Build();

            //app.Map("/time", (TimeService timeService) => $"Time: {timeService.Time}");
            //app.Map("/", () => "Hello METANIT.COM");

            //app.Run();

            //можно получить зависимости, если обработчик маршрута конечной точки вынесен в отдельный метод:
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddTransient<TimeService>();   // Добавляем сервис

            var app = builder.Build();

            app.Map("/time", SendTime);
            app.Map("/", () => "Hello METANIT.COM");

            app.Run();

            string SendTime(TimeService timeService)
            {
                return $"Time: {timeService.Time}";
            }


    }
}
}
