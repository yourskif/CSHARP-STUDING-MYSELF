namespace Отправка_файлов_в_Results_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);
            //var app = builder.Build();

            //app.MapGet("/", () => "Hello World!");

            //app.Run();

            //Отправка файла как массива байтов
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Map("/forest", async () =>
            {
                string path = "Files/forest.png";
                byte[] fileContent = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
                string contentType = "image/png";       // установка mime-типа
                string downloadName = "winter_forest.png";  // установка загружаемого имени
                return Results.File(fileContent, contentType, downloadName);
            });

            app.Map("/", () => "Hello World");

            app.Run();


            //Также, как и в предыдущем примере, отправим файл forest.png из папки Files:
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/forest", () =>
            //{
            //    string path = "Files/forest.png";
            //    FileStream fileStream = new FileStream(path, FileMode.Open);
            //    string contentType = "image/png";
            //    string downloadName = "winter_forest.png";
            //    return Results.File(fileStream, contentType, downloadName);
            //});

            //app.Map("/", () => "Hello World");

            //app.Run();


            //Отправка файла по определенному пути
            //var builder = WebApplication.CreateBuilder();
            //var app = builder.Build();

            //app.Map("/forest", () =>
            //{
            //    string path = "Files/forest.png";
            //    string contentType = "image/png";
            //    string downloadName = "winter_forest.png";
            //    return Results.File(path, contentType, downloadName);
            //});

            //app.Map("/", () => "Hello World");

            //app.Run();


            //Но это поведение мы можем переопределить. Допустим, в проекте есть папка Files. И мы хотим, чтобы приложение автоматически подхватывало из нее файлы
            //Для этого добавим новый каталог для статических файлов:
            //var builder = WebApplication.CreateBuilder(
            //    new WebApplicationOptions { WebRootPath = "Files" });  // добавляем папку для хранения файлов
            //var app = builder.Build();

            //app.Map("/river", () =>
            //{
            //    string path = "newRiver.jpg";
            //    string contentType = "image/jpeg";
            //    string downloadName = "river.jpg";
            //    return Results.File(path, contentType, downloadName);
            //});

            //app.Map("/", () => "Hello World");

            //app.Run();
        }
    }
}
