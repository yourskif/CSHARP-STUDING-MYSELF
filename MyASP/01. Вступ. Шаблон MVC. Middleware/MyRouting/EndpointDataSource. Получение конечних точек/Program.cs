using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace RouteInspectorApp // ← новий унікальний простір імен
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Реєстрація маршрутів
            app.Map("/", () => "Index Page");
            app.Map("/about", () => "About Page");
            app.Map("/contact", () => "Contacts Page");

            // Вивід усіх маршрутів
            app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
            {
                var routes = endpointSources
                    .SelectMany(source => source.Endpoints)
                    .OfType<RouteEndpoint>()
                    .Select(endpoint => endpoint.RoutePattern.RawText);

                return string.Join("\n", routes);
            });

            app.Run();
        }
    }
}



//namespace EndpointDataSource._Получение_конечних_точек
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            //var builder = WebApplication.CreateBuilder(args);
//            //var app = builder.Build();

//            //app.MapGet("/", () => "Hello World!");

//            //app.Run();

//            //Получение всех маршрутів
//            var builder = WebApplication.CreateBuilder();
//            var app = builder.Build();

//            app.Map("/", () => "Index Page");
//            app.Map("/about", () => "About Page");
//            app.Map("/contact", () => "Contacts Page");

//            app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
//                    string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));

//            app.Run();
//        }
//    }
//}
