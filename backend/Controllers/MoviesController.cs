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
        private readonly ICacheService _cacheService;
        private readonly ILogger<MoviesController> _logger;
        private const string RATED_MOVIES_CACHE_KEY = "rated_movies";
        private const string MOVIE_DETAILS_CACHE_PREFIX = "movie_details_";

        public MoviesController(ITMDBService tMDBService, ApplicationDbContext context, ICacheService cacheService, ILogger<MoviesController> logger)
        {
            _tmdbService = tMDBService;
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query)
        {
            try
            {
                // Cache key for search results
                string cacheKey = $"movie_search_{query.ToLower()}";
                
                // Try get from cache first
                var cachedResult = await _cacheService.GetAsync<SearchResult<MovieDto>>(cacheKey);
                if (cachedResult != null)
                {
                    return Ok(cachedResult);
                }

                var result = await _tmdbService.SearchMoviesAsync(query);
                
                // Cache search results for 1 hour
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1));
                
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
                        // Try get movie details from cache
                        string detailsCacheKey = $"{MOVIE_DETAILS_CACHE_PREFIX}{reviewDto.MovieId}";
                        var movieDetails = await _cacheService.GetAsync<MovieDto>(detailsCacheKey);
                        
                        if (movieDetails == null)
                        {
                            movieDetails = await _tmdbService.GetMovieDetailsAsync(reviewDto.MovieId);
                            // Cache movie details for 24 hours
                            await _cacheService.SetAsync(detailsCacheKey, movieDetails, TimeSpan.FromHours(24));
                        }

                        var utcReviewDate = DateTime.UtcNow;
                        var genreIdsString = string.Join(",", movieDetails.GenreIds);

                        existingMovie = new Movie
                        {
                            Id = movieDetails.Id,
                            Title = movieDetails.Title,
                            Overview = movieDetails.Overview,
                            PosterPath = movieDetails.PosterPath,
                            ReleaseDate = movieDetails.ReleaseDate.ToUniversalTime(),
                            Rating = reviewDto.Rating,
                            Comment = reviewDto.Comment,
                            ReviewDate = utcReviewDate,
                            GenreIds = genreIdsString
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
                    existingMovie.ReviewDate = DateTime.UtcNow;
                }

                try {
                    await _context.SaveChangesAsync();
                    await _cacheService.RemoveAsync(RATED_MOVIES_CACHE_KEY);
                    return Ok(existingMovie);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
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
                var cachedMovies = await _cacheService.GetAsync<List<object>>(RATED_MOVIES_CACHE_KEY);
                if (cachedMovies != null)
                {
                    return Ok(cachedMovies);
                }

                var ratedMovies = await _context.Movies
                    .AsNoTracking()
                    .Where(m => !m.IsHidden)
                    .OrderByDescending(m => m.ReviewDate)
                    .Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Overview,
                        m.PosterPath,
                        ReleaseDate = m.ReleaseDate.ToString("dd-MM-yyyy"), // Thay đổi format
                        m.Rating,
                        m.Comment,
                        ReviewDate = m.ReviewDate.ToString("dd-MM-yyyy"), // Thay đổi format
                        m.IsHidden,
                        m.GenreIds,
                    })
                    .ToListAsync();

                if (ratedMovies != null && ratedMovies.Any())
                {
                    await _cacheService.SetAsync(RATED_MOVIES_CACHE_KEY, ratedMovies, TimeSpan.FromMinutes(5));
                }

                return Ok(ratedMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rated movies");
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

                // Clear related caches
                await _cacheService.RemoveAsync(RATED_MOVIES_CACHE_KEY);
                await _cacheService.RemoveAsync($"{MOVIE_DETAILS_CACHE_PREFIX}{id}");

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

                // Clear rated movies cache when visibility changes
                await _cacheService.RemoveAsync(RATED_MOVIES_CACHE_KEY);

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