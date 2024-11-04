using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace backend.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        
        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string ReleaseDateStr { get; set; } = string.Empty;

        public DateTime ReleaseDate
    {
        get
        {
            // TMDB trả về ngày dạng "YYYY-MM-DD"
            if (string.IsNullOrEmpty(ReleaseDateStr))
                return DateTime.MinValue;

            if (DateTime.TryParseExact(ReleaseDateStr, 
                "yyyy-MM-dd", 
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, 
                out DateTime date))
            {
                return date;
            }
            return DateTime.MinValue;
        }
    }
    }

    public class MovieReviewDto
    {
        public int MovieId { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}