using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class MovieGenre
    {
        public int MovieId { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; } = null!;
        
        public int GenreId { get; set; }
        [JsonIgnore]
        public Genre Genre { get; set; } = null!;
    }
} 