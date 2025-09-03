namespace Results_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();
            /*� ������ ������ � ���������� ���������� ��� �������� �����: ���� ������������ ������� �� �������� 
             * "/hello", � ������ - ������� � ����� ���-����������. ������ �������� ����� ��� �������� ������ 
             * ���������� ����� Results.Text(). ������ �������� ����� �������� ���������� ��������� ������. 
             * ������ � ���������� ��� �������� ����� ����������� ����� ����������� �����: ����� ����� 
             * ������������ ���� � �� �� ������ �� ��������� ����� 200. ������� ����� ����������� ������ � 
             * ��������� ����������� (� ���������, ����� Reqults.Text ��������� ��������� "Content-Length").
            */

            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/hello", () => Results.Text("Hello ASP.NET Core"));
            //app.Map("/", () => "Hello ASP.NET Core");

            //app.Run();

            //�������� ������ ����� ������������ ������ � ��������� ������, ������� ���������� ������ IResult:
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/hello", SendHello);
            app.Map("/", () => "Hello ASP.NET Core 6");

            app.Run();

            IResult SendHello()
            {
                return Results.Text("Hello ASP.NET Core");
            }

        }
    }
}
