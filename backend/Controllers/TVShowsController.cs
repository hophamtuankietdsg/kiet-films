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

        public TVShowsController(ITMDBService tMDBService ,ApplicationDbContext context)
        {
            _tmdbService = tMDBService;
            _context = context;
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

        [HttpGet("rated")]
        public async Task<IActionResult> GetRatedTVShows()
        {
            try
            {
                var ratedTVShows = await _context.TVShows
                    .Where(t => !t.IsHidden)
                    .OrderByDescending(t => t.ReviewDate)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Overview,
                        t.PosterPath,
                        FirstAirDate = t.FormattedFirstAirDate,
                        t.Rating,
                        t.Comment,
                        ReviewDate = t.FormattedReviewDate,
                        t.IsHidden
                    })
                    .ToListAsync();
                return Ok(ratedTVShows);
            }
            catch (Exception ex)
            {
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

                return Ok(new { 
                    message = $"Movie '{tvShow.Name}' is now {(tvShow.IsHidden ? "hidden" : "visible")}",
                    isHidden = tvShow.IsHidden
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error toggling TV Show visibility: {ex.Message}");
            }
        }
    }
}