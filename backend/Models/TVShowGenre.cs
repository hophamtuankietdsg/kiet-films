using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class TVShowGenre
    {
        public int TVShowId { get; set; }
        [JsonIgnore]
        public TVShow TVShow { get; set; } = null!;
        
        public int GenreId { get; set; }
        [JsonIgnore]
        public Genre Genre { get; set; } = null!;
    }
} 