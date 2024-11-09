using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => new { x.Id, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Overview = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PosterPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    GenreIds = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TVShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Overview = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PosterPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FirstAirDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    GenreIds = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVShows", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Type", "Name" },
                values: new object[,]
                {
                    { 12, "movie", "Adventure" },
                    { 14, "movie", "Fantasy" },
                    { 16, "movie", "Animation" },
                    { 16, "tv", "Animation" },
                    { 18, "movie", "Drama" },
                    { 18, "tv", "Drama" },
                    { 27, "movie", "Horror" },
                    { 28, "movie", "Action" },
                    { 35, "movie", "Comedy" },
                    { 35, "tv", "Comedy" },
                    { 36, "movie", "History" },
                    { 37, "movie", "Western" },
                    { 37, "tv", "Western" },
                    { 53, "movie", "Thriller" },
                    { 80, "movie", "Crime" },
                    { 80, "tv", "Crime" },
                    { 99, "movie", "Documentary" },
                    { 99, "tv", "Documentary" },
                    { 878, "movie", "Science Fiction" },
                    { 9648, "movie", "Mystery" },
                    { 9648, "tv", "Mystery" },
                    { 10402, "movie", "Music" },
                    { 10749, "movie", "Romance" },
                    { 10751, "movie", "Family" },
                    { 10751, "tv", "Family" },
                    { 10752, "movie", "War" },
                    { 10759, "tv", "Action & Adventure" },
                    { 10762, "tv", "Kids" },
                    { 10763, "tv", "News" },
                    { 10764, "tv", "Reality" },
                    { 10765, "tv", "Sci-Fi & Fantasy" },
                    { 10766, "tv", "Soap" },
                    { 10767, "tv", "Talk" },
                    { 10768, "tv", "War & Politics" },
                    { 10770, "movie", "TV Movie" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "TVShows");
        }
    }
}
