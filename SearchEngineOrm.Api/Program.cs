using System.Reflection;
using OrmLibrary.Extensions;
using OrmLibrary.SqlServer;
using SearchEngineOrm.Api.Controllers;
using SearchEngineOrm.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOrmStartup(
    "data source=DESKTOP-GABI;initial catalog=TestDb;trusted_connection=true",
    Assembly.GetAssembly(typeof(QueriesController))!,
    new[] { Assembly.GetAssembly(typeof(Song))! }
).UseSqlServer();

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

app.UseOrmMappings(app.Environment);

app.Run();