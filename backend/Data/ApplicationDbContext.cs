using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        private const int TV_GENRE_ID_OFFSET = 100000;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<TVShow> TVShows { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<MovieGenre> MovieGenres { get; set; } = null!;
        public DbSet<TVShowGenre> TVShowGenres { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Genre primary key
            modelBuilder.Entity<Genre>()
                .HasKey(g => g.Id);

            // Configure Movie-Genre relationship
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TVShow-Genre relationship
            modelBuilder.Entity<TVShowGenre>()
                .HasKey(tg => new { tg.TVShowId, tg.GenreId });

            modelBuilder.Entity<TVShowGenre>()
                .HasOne(tg => tg.TVShow)
                .WithMany(t => t.TVShowGenres)
                .HasForeignKey(tg => tg.TVShowId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TVShowGenre>()
                .HasOne(tg => tg.Genre)
                .WithMany(g => g.TVShowGenres)
                .HasForeignKey(tg => tg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed genre data
            var movieGenres = new List<Genre>
            {
                new Genre { Id = 28, Name = "Action", Type = "movie" },
                new Genre { Id = 12, Name = "Adventure", Type = "movie" },
                new Genre { Id = 16, Name = "Animation", Type = "movie" },
                new Genre { Id = 35, Name = "Comedy", Type = "movie" },
                new Genre { Id = 80, Name = "Crime", Type = "movie" },
                new Genre { Id = 99, Name = "Documentary", Type = "movie" },
                new Genre { Id = 18, Name = "Drama", Type = "movie" },
                new Genre { Id = 10751, Name = "Family", Type = "movie" },
                new Genre { Id = 14, Name = "Fantasy", Type = "movie" },
                new Genre { Id = 36, Name = "History", Type = "movie" },
                new Genre { Id = 27, Name = "Horror", Type = "movie" },
                new Genre { Id = 10402, Name = "Music", Type = "movie" },
                new Genre { Id = 9648, Name = "Mystery", Type = "movie" },
                new Genre { Id = 10749, Name = "Romance", Type = "movie" },
                new Genre { Id = 878, Name = "Science Fiction", Type = "movie" },
                new Genre { Id = 10770, Name = "TV Movie", Type = "movie" },
                new Genre { Id = 53, Name = "Thriller", Type = "movie" },
                new Genre { Id = 10752, Name = "War", Type = "movie" },
                new Genre { Id = 37, Name = "Western", Type = "movie" }
            };

            var tvGenres = new List<Genre>
            {
                new Genre { Id = TV_GENRE_ID_OFFSET + 10759, Name = "Action & Adventure", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 16, Name = "Animation", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 35, Name = "Comedy", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 80, Name = "Crime", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 99, Name = "Documentary", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 18, Name = "Drama", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10751, Name = "Family", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10762, Name = "Kids", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 9648, Name = "Mystery", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10763, Name = "News", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10764, Name = "Reality", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10765, Name = "Sci-Fi & Fantasy", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10766, Name = "Soap", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10767, Name = "Talk", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 10768, Name = "War & Politics", Type = "tv" },
                new Genre { Id = TV_GENRE_ID_OFFSET + 37, Name = "Western", Type = "tv" }
            };

            var allGenres = movieGenres.Concat(tvGenres);
            modelBuilder.Entity<Genre>().HasData(allGenres);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}