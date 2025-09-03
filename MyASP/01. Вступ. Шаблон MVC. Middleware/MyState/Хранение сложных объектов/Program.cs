using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Хранение_сложных_объектов;

var builder = WebApplication.CreateBuilder(args);

// Додати підтримку сесій
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseSession();

app.Run(async context =>
{
    if (context.Session.Keys.Contains("person"))
    {
        var person = context.Session.Get<Person>("person");
        await context.Response.WriteAsync($"Hello {person?.Name}, your age: {person?.Age}!");
    }
    else
    {
        var person = new Person { Name = "Tom", Age = 22 };
        context.Session.Set("person", person);
        await context.Response.WriteAsync("Hello World!");
    }
});
