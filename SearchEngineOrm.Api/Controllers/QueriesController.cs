using Microsoft.AspNetCore.Mvc;
using OrmLibrary.Execution;
using SearchEngineOrm.Domain.Entities;

namespace SearchEngineOrm.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueriesController : ControllerBase
    {
        [HttpGet]
        [Route("/Songs/Title")]
        public IActionResult GetSongsWithTitle([FromQuery] string? start, [FromQuery] string? end, [FromQuery] string? contains)
        {
            using var context = new ScopedDbContext();

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
        
        
    }
}
