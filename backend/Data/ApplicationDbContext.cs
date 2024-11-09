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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever(); // ID sẽ không tự động generate
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Overview).HasMaxLength(2000);
                entity.Property(e => e.PosterPath).HasMaxLength(500);
                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.GenreIds).HasMaxLength(100);
            });

            modelBuilder.Entity<TVShow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Overview).HasMaxLength(2000);
                entity.Property(e => e.PosterPath).HasMaxLength(500);
                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.GenreIds).HasMaxLength(100);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type });
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(10);
            });

            // Seed genre data
            var genres = new List<Genre>
            {
                // Movie Genres
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
                new Genre { Id = 37, Name = "Western", Type = "movie" },

                // TV Show Genres
                new Genre { Id = 10759, Name = "Action & Adventure", Type = "tv" },
                new Genre { Id = 16, Name = "Animation", Type = "tv" },
                new Genre { Id = 35, Name = "Comedy", Type = "tv" },
                new Genre { Id = 80, Name = "Crime", Type = "tv" },
                new Genre { Id = 99, Name = "Documentary", Type = "tv" },
                new Genre { Id = 18, Name = "Drama", Type = "tv" },
                new Genre { Id = 10751, Name = "Family", Type = "tv" },
                new Genre { Id = 10762, Name = "Kids", Type = "tv" },
                new Genre { Id = 9648, Name = "Mystery", Type = "tv" },
                new Genre { Id = 10763, Name = "News", Type = "tv" },
                new Genre { Id = 10764, Name = "Reality", Type = "tv" },
                new Genre { Id = 10765, Name = "Sci-Fi & Fantasy", Type = "tv" },
                new Genre { Id = 10766, Name = "Soap", Type = "tv" },
                new Genre { Id = 10767, Name = "Talk", Type = "tv" },
                new Genre { Id = 10768, Name = "War & Politics", Type = "tv" },
                new Genre { Id = 37, Name = "Western", Type = "tv" }
            };

            modelBuilder.Entity<Genre>().HasData(genres);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Enable second level caching
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}