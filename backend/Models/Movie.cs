using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Overview { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PosterPath { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime ReleaseDate { get; set; }

        [NotMapped]
        public string FormattedReleaseDate => ReleaseDate != DateTime.MinValue 
            ? ReleaseDate.ToLocalTime().ToString("dd/MM/yyyy") 
            : "Chưa có ngày";
        public double Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; }

        [NotMapped]
        public string FormattedReviewDate => ReviewDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

        public bool IsHidden { get; set; }
    }
}