using System;

namespace RoutingMiddleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //RoutingMiddleware
            //����� �������, ���� �� ������ �������� ������ � ��������� �� ���� "/index" ��� "/about"
            //� �� ��������� �������� token, �� �� ������� ������. ���� �� ��������� �� ���� / index
            //��� / about � ��������� �������� ��������� token, �� ������ ������� �����:
            //https://localhost:7083/index?token=12345
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<RoutingMiddleware>();

            app.Run();
        }
    }
}
