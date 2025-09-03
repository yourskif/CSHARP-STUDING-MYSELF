namespace Получение_зависимостей
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();


            var builder = WebApplication.CreateBuilder();

            builder.Services.AddTransient<ITimeService, ShortTimeService>();

            var app = builder.Build();
            //GetService
            //app.Run(async context =>
            //{
            //    var timeService = app.Services.GetService<ITimeService>();
            //    await context.Response.WriteAsync($"Time: {timeService?.GetTime()}");
            //});
            //GetRequiredService
            //app.Run(async context =>
            //{
            //    var timeService = app.Services.GetRequiredService<ITimeService>();
            //    await context.Response.WriteAsync($"Time: {timeService.GetTime()}");
            //});

            //HttpContext.RequestServices
            app.Run(async context =>
            {
                var timeService = context.RequestServices.GetService<ITimeService>();
                await context.Response.WriteAsync($"Time: {timeService?.GetTime()}");
            });


            app.Run();


    }
    }
}
