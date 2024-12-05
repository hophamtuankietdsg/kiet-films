using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TVShowsController : ControllerBase
    {
        private readonly ITMDBService _tmdbService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TVShowsController> _logger;

        public TVShowsController(ITMDBService tMDBService ,ApplicationDbContext context, ILogger<TVShowsController> logger)
        {
            _tmdbService = tMDBService;
            _context = context;
            _logger = logger;
        }

        private IActionResult HandleError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
            return StatusCode(500, ex.Message);
        }

        // GET: api/tvshows/search?query={query}
        [HttpGet("search")]
        public async Task<IActionResult> SearchTVShows([FromQuery] string query)
        {
            try
            {
                var result = await _tmdbService.SearchTVShowsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error searching TV shows");
            }
        }

        // POST: api/tvshows/rate
        [HttpPost("rate")]
        public async Task<IActionResult> RateTVShow([FromBody] TVShowReviewDto reviewDto)
        {
            try
            {
                var existingTVShow = await _context.TVShows
                    .FirstOrDefaultAsync(t => t.Id == reviewDto.TVShowId);
                
                if (existingTVShow == null)
                {
                    var tvShowDetails = await _tmdbService.GetTVShowDetailsAsync(reviewDto.TVShowId);
                    var utcReviewDate = DateTime.UtcNow;
                    var genreIdsString = string.Join(",", tvShowDetails.GenreIds);

                    existingTVShow = new TVShow
                    {
                        Id = tvShowDetails.Id,
                        Name = tvShowDetails.Name,
                        Overview = tvShowDetails.Overview,
                        PosterPath = tvShowDetails.PosterPath,
                        FirstAirDate = tvShowDetails.FirstAirDate.ToUniversalTime(),
                        Rating = reviewDto.Rating,
                        Comment = reviewDto.Comment,
                        ReviewDate = utcReviewDate,
                        GenreIds = genreIdsString
                    };

                    await _context.TVShows.AddAsync(existingTVShow);
                }
                else
                {
                    existingTVShow.Rating = reviewDto.Rating;
                    existingTVShow.Comment = reviewDto.Comment;
                    existingTVShow.ReviewDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return Ok(existingTVShow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/tvshows/rated
        [HttpGet("rated")]
        public async Task<IActionResult> GetRatedTVShows()
        {
            try
            {
                var ratedTVShows = await _context.TVShows
                    .AsNoTracking()
                    .Where(t => !t.IsHidden)
                    .OrderByDescending(t => t.ReviewDate)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Overview,
                        t.PosterPath,
                        FirstAirDate = t.FirstAirDate.ToString("dd/MM/yyyy"),
                        t.Rating,
                        t.Comment,
                        ReviewDate = t.ReviewDate.ToString("dd/MM/yyyy"),
                        t.IsHidden,
                        t.GenreIds,
                    })
                    .ToListAsync();

                return Ok(ratedTVShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rated TV shows");
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/tvshows/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTVShow(int id)
        {
            try
            {
                var tvShow = await _context.TVShows.FindAsync(id);
                if (tvShow == null)
                {
                    return NotFound($"TV Show with ID {id} not found!");
                }

                _context.TVShows.Remove(tvShow);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"TV Show '{tvShow.Name}' deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting TV Show: {ex.Message}");
            }
        }

        // PATCH: api/tvshows/{id}/visibility
        [HttpPatch("{id}/visibility")]
        public async Task<IActionResult> ToggleTVShowVisibility(int id)
        {
            try
            {
                var tvShow = await _context.TVShows
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tvShow == null)
                {
                    return NotFound($"TV Show with ID {id} not found!");
                }

                var updatedTVShow = new TVShow
                {
                    Id = tvShow.Id,
                    Name = tvShow.Name,
                    Overview = tvShow.Overview,
                    PosterPath = tvShow.PosterPath,
                    FirstAirDate = tvShow.FirstAirDate,
                    Rating = tvShow.Rating,
                    Comment = tvShow.Comment,
                    ReviewDate = tvShow.ReviewDate,
                    GenreIds = tvShow.GenreIds,
                    IsHidden = !tvShow.IsHidden
                };

                _context.TVShows.Update(updatedTVShow);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"TV Show '{tvShow.Name}' is now {(tvShow.IsHidden ? "hidden" : "visible")}",
                    isHidden = updatedTVShow.IsHidden
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error toggling TV Show visibility: {ex.Message}");
            }
        }

        // GET: api/tvshows/{id}/videos
        [HttpGet("{id}/videos")]
        public async Task<IActionResult> GetTVShowVideo(int id)
        {
            try 
            {
                var videos = await _tmdbService.GetTVShowVideosAsync(id);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error getting TV show videos");
            }
        }
    }
}