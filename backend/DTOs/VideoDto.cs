using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace backend.DTOs
{
    public class VideoDto
    {
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("iso_639_1")]
        public string Iso639_1 { get; set; }= string.Empty;

        [JsonPropertyName("iso_3166_1")]
        public string Iso3166_1 { get; set; }= string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        public int Size { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool Official { get; set; }

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; } = string.Empty;
    }

    public class VideoResponse
    {
        public int Id { get; set; }
        public List<VideoDto> Results { get; set; } = new();
    }
}