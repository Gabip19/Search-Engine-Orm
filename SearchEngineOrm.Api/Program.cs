using System.Reflection;
using OrmLibrary;
using OrmLibrary.Execution;
using OrmLibrary.Extensions;
using SearchEngineOrm.Api;
using SearchEngineOrm.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOrmStartup(
    Assembly.GetAssembly(typeof(WeatherForecast))!,
    new[] { Assembly.GetAssembly(typeof(Song))! }
);

var app = builder.Build();

Console.WriteLine("Dev environment: " + app.Environment.IsDevelopment());

var context = new ScopedDbContext();
var a = context.Entity<Song>().Query()
    .Select(song => new Artist
    {
        Name = song.SongTitle, 
        Description = song.ArtistName
    })
    .Where(song => (song.SongTitle.StartsWith("asd") && (song.ArtistName == "test" && song.PopularityScore == 3f)) || song.IsExplicit == null)
    .OrderBy(song => song.SongTitle)
    .OrderByDescending(song => song.ArtistName)
    .Skip(10)
    .Take(10);

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
