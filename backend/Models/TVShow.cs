using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class TVShow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Overview { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PosterPath { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime FirstAirDate { get; set; }

        [NotMapped]
        public string FormattedFirstAirDate => FirstAirDate != DateTime.MinValue
            ? FirstAirDate.ToLocalTime().ToString("dd/MM/yyyy")
            : "Chưa có ngày";
        
        public double Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; }

        [NotMapped]
        public string FormattedReviewDate => ReviewDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        public bool IsHidden { get; set; }

        public string GenreIds { get; set; } = string.Empty;

        [NotMapped]
        public List<int> Genres
        {
            get
            {
                if (string.IsNullOrEmpty(GenreIds))
                    return new List<int>();
                return GenreIds.Split(',').Select(int.Parse).ToList();
            }
            set
            {
                GenreIds = string.Join(",", value);
            }
        }

        public ICollection<TVShowGenre> TVShowGenres { get; set; } = new List<TVShowGenre>();
    }
}