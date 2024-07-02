using System.Reflection;
using OrmLibrary.Execution;
using OrmLibrary.Extensions;
using OrmLibrary.SqlServer;
using SearchEngineOrm.Api;
using SearchEngineOrm.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISqlQueryGenerator, SqlServerQueryGenerator>();
builder.Services.AddTransient<ScopedDbContext>();

builder.Services.ConfigureOrmStartup(
    "data source=DESKTOP-GABI;initial catalog=TestDb;trusted_connection=true",
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

app.UseOrmMappings(app.Environment);

// app.Run();

var ceva = "app";
var id = Guid.NewGuid();
var arrayStr = new[] { "set", "chech" };
var arrayInt = new[] { 1, 2, 3 };

var context = new ScopedDbContext();
context.Entity<Song>().Query()
    .Select(song => new
    {
        song.SongTitle,
        song.ArtistName
    })
    .Where(song => ((song.SongTitle.StartsWith("name_artist") && song.TrackId == 3) || song.SongTitle != null) && song.SongTitle.Contains(ceva) || arrayInt.Contains(song.TrackId))
    .Load(song => song.MainArtist)
    .OrderBy(song => song.SongTitle)
    .OrderByDescending(song => song.SongTitle)
    .Skip(10)
    .Take(10)
    .Count()
    .Execute();

Console.WriteLine("Done");