using Microsoft.AspNetCore.Mvc;
using OrmLibrary.Abstractions;
using SearchEngineOrm.Domain.Entities;

namespace SearchEngineOrm.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class QueriesController : ControllerBase
{
    private readonly IDbContextFactory _dbContextFactory;
    
    public QueriesController(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    [HttpGet]
    [Route("/Songs/Title")]
    public IActionResult GetSongsWithTitle([FromQuery] string? start, [FromQuery] string? end, [FromQuery] string? contains)
    {
        using var context = _dbContextFactory.CreateContext();

        var query = context.Entity<Song>().Query();
            
        if (start is not null)
        {
            query.Where(song => song.SongTitle.StartsWith(start));
        }
        else if (end is not null)
        {
            query.Where(song => song.SongTitle.EndsWith(end));
        }
        else if (contains is not null)
        {
            query.Where(song => song.SongTitle.Contains(contains));
        }

        var result = query
            .OrderBy(song => song.PopularityScore)
            .Take(5)
            .Execute();

        return Ok(result);
    }
        
    [HttpGet]
    [Route("/Artist/Count")]
    public IActionResult GetArtistNumber()
    {
        using var context = _dbContextFactory.CreateContext();

        var result = context.Entity<Artist>().Query()
            .Where(artist => artist.SongsNum > 60)
            .Count();

        return Ok(result);
    }

    [HttpGet]
    [Route("/Artist/Names")]
    public IActionResult GetArtistNames([FromQuery] string contains)
    {
        using var context = _dbContextFactory.CreateContext();

        var result = context.Entity<Artist>().Query()
            .Select(artist => new
            {
                artist.StageName
            })
            .Where(artist => artist.SongsNum > 40 && artist.StageName.Contains(contains))
            .OrderByDescending(artist => artist.StageName)
            .Execute();

        return Ok(result);
    }
    
    [HttpGet]
    [Route("/Songs/Average")]
    public IActionResult GetSongsFiltered()
    {
        using var context = _dbContextFactory.CreateContext();

        var result = context.Entity<Song>().Query()
            .Where(song => song.PopularityScore > 10 || (song.RadioAppearances != null && song.RadioAppearances < 100))
            .Average(song => song.PopularityScore);

        return Ok(result);
    }
}