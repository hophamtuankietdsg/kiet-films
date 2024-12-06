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
        private const int TV_GENRE_ID_OFFSET = 100000;
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

        [HttpPost("rate")]
        public async Task<IActionResult> RateTVShow([FromBody] TVShowReviewDto reviewDto)
        {
            try
            {
                var existingTVShow = await _context.TVShows
                    .Include(t => t.TVShowGenres)
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
                        GenreIds = genreIdsString,
                        TVShowGenres = tvShowDetails.GenreIds.Select(genreId => new TVShowGenre
                        {
                            TVShowId = tvShowDetails.Id,
                            GenreId = TV_GENRE_ID_OFFSET + genreId
                        }).ToList()
                    };

                    await _context.TVShows.AddAsync(existingTVShow);
                }
                else
                {
                    existingTVShow.Rating = reviewDto.Rating;
                    existingTVShow.Comment = reviewDto.Comment;
                    existingTVShow.ReviewDate = DateTime.UtcNow;

                    // Cập nhật TVShowGenres
                    var tvShowDetails = await _tmdbService.GetTVShowDetailsAsync(reviewDto.TVShowId);
                    existingTVShow.GenreIds = string.Join(",", tvShowDetails.GenreIds);
                    
                    // Xóa các genre cũ
                    _context.TVShowGenres.RemoveRange(existingTVShow.TVShowGenres);
                    
                    // Thêm các genre mới với offset
                    existingTVShow.TVShowGenres = tvShowDetails.GenreIds.Select(genreId => new TVShowGenre
                    {
                        TVShowId = tvShowDetails.Id,
                        GenreId = TV_GENRE_ID_OFFSET + genreId
                    }).ToList();
                }

                await _context.SaveChangesAsync();
                return Ok(existingTVShow);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error rating TV show");
            }
        }

        [HttpGet("rated")]
        public async Task<IActionResult> GetRatedTVShows()
        {
            try
            {
                var ratedTVShows = await _context.TVShows
                    .Include(t => t.TVShowGenres)
                    .ThenInclude(tg => tg.Genre)
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
                        TVShowGenres = t.TVShowGenres.Select(tg => new
                        {
                            GenreId = tg.GenreId - TV_GENRE_ID_OFFSET, // Remove offset for client
                            tg.Genre.Name,
                            tg.Genre.Type
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(ratedTVShows);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error getting rated TV shows");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTVShow(int id)
        {
            try
            {
                var tvShow = await _context.TVShows
                    .Include(t => t.TVShowGenres)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tvShow == null)
                {
                    return NotFound($"TV Show with ID {id} not found!");
                }

                _context.TVShowGenres.RemoveRange(tvShow.TVShowGenres);
                _context.TVShows.Remove(tvShow);
                await _context.SaveChangesAsync();

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

                // Toggle the IsHidden value
                tvShow.IsHidden = !tvShow.IsHidden;
                _context.Entry(tvShow).Property(x => x.IsHidden).IsModified = true;
                await _context.SaveChangesAsync();

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