using System.Reflection;
using OrmLibrary;
using OrmLibrary.Extensions;
using SearchEngineOrm.Api;
using SearchEngineOrm.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOrmStartup(
    Assembly.GetAssembly(typeof(WeatherForecast))!,
    new[] { Assembly.GetAssembly(typeof(Song))! }
);

var app = builder.Build();

Console.WriteLine("Dev environment: " + app.Environment.IsDevelopment());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.Run();
