using MassTransit; // Добавлено для MassTransit
using Microsoft.AspNetCore.Mvc;
using PostService.Application.Consumers; // Добавлено для CreatePostConsumer
using PostService.Application.Interfaces.IServices; // Для IPostService
using PostService.Domain.Entities; // Для Post
using Shared.Messages; // Добавлено для сообщений SAGA

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- НАЧАЛО: Настройка MassTransit ---
builder.Services.AddMassTransit(x =>
{
    // Регистрация Consumer'ов для PostService
    x.AddConsumer<CreatePostConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => // Замените "localhost" на адрес вашего RabbitMQ
        {
            h.Username("guest"); // Замените на реальные учетные данные
            h.Password("guest");
        });

        // Конфигурация для обработки команды создания анкеты
        cfg.ReceiveEndpoint("create-post-command", e =>
        {
            e.ConfigureConsumer<CreatePostConsumer>(context);
        });
    });
});
// --- КОНЕЦ: Настройка MassTransit ---

// Регистрация IPostService (замените на вашу реальную реализацию)
// builder.Services.AddScoped<IPostService, YourPostServiceImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}