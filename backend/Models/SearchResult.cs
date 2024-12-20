using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTOs;

namespace backend.Models
{
    public class SearchResult<T>
    {
        public int Page { get; set; }
        public List<T> Results { get; set; } = new();
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }
}