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
        private readonly ICacheService _cacheService;
        private readonly ILogger<MoviesController> _logger;
        private const string RATED_TVSHOWS_CACHE_KEY = "rated_tvshows";

        public TVShowsController(ITMDBService tMDBService ,ApplicationDbContext context, ICacheService cacheService, ILogger<MoviesController> logger)
        {
            _tmdbService = tMDBService;
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("rated")]
        public async Task<IActionResult> RateTVShows([FromBody] TVShowReviewDto reviewDto)
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
                
                // Xóa cache để buộc refresh
                await _cacheService.RemoveAsync(RATED_TVSHOWS_CACHE_KEY);
                
                return Ok(existingTVShow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("rated")]
        public async Task<IActionResult> GetRatedTVShows()
        {
            try
            {
                var cachedTVShows = await _cacheService.GetAsync<List<object>>(RATED_TVSHOWS_CACHE_KEY);
                if (cachedTVShows != null)
                {
                    return Ok(cachedTVShows);
                }

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
                        FirstAirDate = t.FirstAirDate.ToString("dd/MM/yyyy"), // Thay đổi format
                        t.Rating,
                        t.Comment,
                        ReviewDate = t.ReviewDate.ToString("dd/MM/yyyy"), // Thay đổi format
                        t.IsHidden,
                        t.GenreIds,
                    })
                    .ToListAsync();

                if (ratedTVShows != null && ratedTVShows.Any())
                {
                    await _cacheService.SetAsync(RATED_TVSHOWS_CACHE_KEY, ratedTVShows, TimeSpan.FromMinutes(5));
                }

                return Ok(ratedTVShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rated TV shows");
                return StatusCode(500, ex.Message);
            }
        }

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

                // Xóa cache khi xóa TV Show
                await _cacheService.RemoveAsync(RATED_TVSHOWS_CACHE_KEY);

                return Ok(new { message = $"TV Show '{tvShow.Name}' deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting TV Show: {ex.Message}");
            }
        }

        [HttpPatch("{id}/visibility")]
        public async Task<IActionResult> ToggleTVShowVisibility(int id)
        {
            try
            {
                var tvShow = await _context.TVShows.FindAsync(id);
                if (tvShow == null)
                {
                    return NotFound($"TV Show with ID {id} not found!");
                }

                tvShow.IsHidden = !tvShow.IsHidden;
                await _context.SaveChangesAsync();

                // Xóa cache khi thay đổi visibility
                await _cacheService.RemoveAsync(RATED_TVSHOWS_CACHE_KEY);

                return Ok(new { 
                    message = $"TV Show '{tvShow.Name}' is now {(tvShow.IsHidden ? "hidden" : "visible")}",
                    isHidden = tvShow.IsHidden
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error toggling TV Show visibility: {ex.Message}");
            }
        }

        [HttpGet("test-cache")]
        public async Task<IActionResult> TestCache()
        {
            try
            {
                string testKey = "test_cache_key";
                string testValue = $"Test value at {DateTime.UtcNow}";

                // Set cache
                await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromMinutes(1));
                
                // Get cache
                var cachedValue = await _cacheService.GetAsync<string>(testKey);

                return Ok(new
                {
                    originalValue = testValue,
                    cachedValue = cachedValue,
                    isCacheWorking = testValue == cachedValue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis test failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}