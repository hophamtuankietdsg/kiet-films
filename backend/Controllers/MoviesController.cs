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
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ITMDBService tMDBService, ApplicationDbContext context, ILogger<MoviesController> logger)
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
        public async Task<IActionResult> SearchMovies([FromQuery] string query)
        {
            try
            {
                var result = await _tmdbService.SearchMoviesAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error searching movies");
            }
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateMovie([FromBody] MovieReviewDto reviewDto)
        {
            try
            {
                var existingMovie = await _context.Movies
                    .Include(m => m.MovieGenres)
                    .FirstOrDefaultAsync(m => m.Id == reviewDto.MovieId);
                
                if (existingMovie == null)
                {
                    var movieDetails = await _tmdbService.GetMovieDetailsAsync(reviewDto.MovieId);
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
                        GenreIds = genreIdsString,
                        MovieGenres = movieDetails.GenreIds.Select(genreId => new MovieGenre
                        {
                            MovieId = movieDetails.Id,
                            GenreId = genreId
                        }).ToList()
                    };

                    await _context.Movies.AddAsync(existingMovie);
                }
                else
                {
                    existingMovie.Rating = reviewDto.Rating;
                    existingMovie.Comment = reviewDto.Comment;
                    existingMovie.ReviewDate = DateTime.UtcNow;

                    var movieDetails = await _tmdbService.GetMovieDetailsAsync(reviewDto.MovieId);
                    existingMovie.GenreIds = string.Join(",", movieDetails.GenreIds);
                    
                    _context.MovieGenres.RemoveRange(existingMovie.MovieGenres);
                    
                    existingMovie.MovieGenres = movieDetails.GenreIds.Select(genreId => new MovieGenre
                    {
                        MovieId = movieDetails.Id,
                        GenreId = genreId
                    }).ToList();
                }

                await _context.SaveChangesAsync();
                return Ok(existingMovie);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error rating movie");
            }
        }

        [HttpGet("rated")]
        public async Task<IActionResult> GetRatedMovies()
        {
            try
            {
                var ratedMovies = await _context.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .AsNoTracking()
                    .Where(m => !m.IsHidden)
                    .OrderByDescending(m => m.ReviewDate)
                    .Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Overview,
                        m.PosterPath,
                        ReleaseDate = m.ReleaseDate.ToString("dd/MM/yyyy"),
                        m.Rating,
                        m.Comment,
                        ReviewDate = m.ReviewDate.ToString("dd/MM/yyyy"),
                        m.IsHidden,
                        m.GenreIds,
                        MovieGenres = m.MovieGenres.Select(mg => new
                        {
                            mg.GenreId,
                            mg.Genre.Name,
                            mg.Genre.Type
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(ratedMovies);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error getting rated movies");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieGenres)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    return NotFound($"Movie with ID {id} not found");
                }

                _context.MovieGenres.RemoveRange(movie.MovieGenres);
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Movie '{movie.Title}' deleted successfully!" });
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

                // Toggle the IsHidden value
                movie.IsHidden = !movie.IsHidden;
                _context.Entry(movie).Property(x => x.IsHidden).IsModified = true;
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

        [HttpGet("{id}/videos")]
        public async Task<IActionResult> GetMovieVideo(int id)
        {
            try 
            {
                var videos = await _tmdbService.GetMovieVideosAsync(id);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error getting movie videos");
            }
        }
    }
}