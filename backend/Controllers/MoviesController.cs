using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                    // Fetch movie details from TMDB
                    var movieDetails = await _tmdbService.GetMovieDetailsAsync(reviewDto.MovieId);

                    existingMovie = new Movie
                    {
                        Id = movieDetails.Id,
                        Title = movieDetails.Title,
                        Overview = movieDetails.Overview,
                        PosterPath = movieDetails.PosterPath,
                        ReleaseDate = movieDetails.ReleaseDate,
                        Rating = reviewDto.Rating,
                        Comment = reviewDto.Comment,
                        // UTC+7
                        ReviewDate = DateTime.UtcNow.AddHours(7),
                    };

                    await _context.Movies.AddAsync(existingMovie);
                }
                else
                {
                    existingMovie.Rating = reviewDto.Rating;
                    existingMovie.Comment = reviewDto.Comment;
                    existingMovie.ReviewDate = DateTime.UtcNow.AddHours(7);
                }

                await _context.SaveChangesAsync();
                return Ok(existingMovie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                    })
                    .ToListAsync();
                return Ok(ratedMovies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}