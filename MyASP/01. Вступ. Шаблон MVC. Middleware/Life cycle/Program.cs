namespace Life_cycle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //AddTransient
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddTransient<ICounter, RandomCounter>();
            builder.Services.AddTransient<CounterService>();

            var app = builder.Build();

            app.UseMiddleware<CounterMiddleware>();

            app.Run();

            //AddScoped
            //var builder = WebApplication.CreateBuilder();

            //builder.Services.AddScoped<ICounter, RandomCounter>();
            //builder.Services.AddScoped<CounterService>();

            //var app = builder.Build();

            //app.UseMiddleware<CounterMiddleware>();

            //app.Run();

            //AddSingleton
            //var builder = WebApplication.CreateBuilder();

            //builder.Services.AddSingleton<ICounter, RandomCounter>();
            //builder.Services.AddSingleton<CounterService>();

            //var app = builder.Build();

            //app.UseMiddleware<CounterMiddleware>();

            //app.Run();

        }
    }
}
