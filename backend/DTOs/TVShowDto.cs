using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace backend.DTOs
{
    public class TVShowDto
    {
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("first_air_date")]
        public string FirstAirDateStr { get; set; } = string.Empty;

        public DateTime FirstAirDate
        {
            get
            {
                if (string.IsNullOrEmpty(FirstAirDateStr))
                    return DateTime.MinValue;
                
                if (DateTime.TryParseExact(FirstAirDateStr,
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

    public class TVShowReviewDto
    {
        public int TVShowId { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}