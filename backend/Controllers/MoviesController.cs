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
    public class MoviesController : ControllerBase
    {
        private readonly ITMDBService _tmdbService;
        private readonly ApplicationDbContext _context;

        public MoviesController(ITMDBService tMDBService, ApplicationDbContext context)
        {
            _tmdbService = tMDBService;
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query)
        {
            try
            {
                var result = await _tmdbService.SearchMoviesAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateMovie([FromBody] MovieReviewDto reviewDto)
        {
            try
            {
                var existingMovie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.Id == reviewDto.MovieId);
                
                if (existingMovie == null)
                {
                    try {
                        var movieDetails = await _tmdbService.GetMovieDetailsAsync(reviewDto.MovieId);

                        // Chuyển đổi thời gian local sang UTC trước khi lưu
                        var utcReviewDate = DateTime.UtcNow;

                        existingMovie = new Movie
                        {
                            Id = movieDetails.Id,
                            Title = movieDetails.Title,
                            Overview = movieDetails.Overview,
                            PosterPath = movieDetails.PosterPath,
                            ReleaseDate = movieDetails.ReleaseDate.ToUniversalTime(), // Chuyển sang UTC
                            Rating = reviewDto.Rating,
                            Comment = reviewDto.Comment,
                            ReviewDate = utcReviewDate, // Sử dụng UTC time
                        };

                        await _context.Movies.AddAsync(existingMovie);
                    }
                    catch (Exception tmdbEx)
                    {
                        return StatusCode(500, $"TMDB Error: {tmdbEx.Message}");
                    }
                }
                else
                {
                    existingMovie.Rating = reviewDto.Rating;
                    existingMovie.Comment = reviewDto.Comment;
                    existingMovie.ReviewDate = DateTime.UtcNow; // Sử dụng UTC time
                }

                try {
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    var innerMessage = dbEx.InnerException?.Message ?? "No inner exception";
                    return StatusCode(500, $"Database Error: {dbEx.Message} - Inner Exception: {innerMessage}");
                }
                
                return Ok(existingMovie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"General Error: {ex.Message} - Stack Trace: {ex.StackTrace}");
            }
        }

        [HttpGet("rated")]
        public async Task<IActionResult> GetRatedMovies()
        {
            try
            {
                var ratedMovies = await _context.Movies
                    .OrderByDescending(m => m.ReviewDate)
                    .Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Overview,
                        m.PosterPath,
                        ReleaseDate = m.FormattedReleaseDate,
                        m.Rating,
                        m.Comment,
                        ReviewDate = m.FormattedReviewDate,
                        m.IsHidden
                    })
                    .ToListAsync();
                return Ok(ratedMovies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found");
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return Ok(new {message = $"Movie '{movie.Title}' deleted successfully!"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting movie: {ex.Message}");
            }
        }

        [HttpPatch("{id}/visibility")]
        public async Task<IActionResult> ToggleMovieVisibility(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found!");
                }

                movie.IsHidden = !movie.IsHidden;
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Movie '{movie.Title}' is now {(movie.IsHidden ? "hidden" : "visible")}",
                    isHidden = movie.IsHidden
                });
            }
            catch (Exception ex)
            {
               return StatusCode(500, $"Error toggling movie visibility: {ex.Message}");
            }
        }
    }
}