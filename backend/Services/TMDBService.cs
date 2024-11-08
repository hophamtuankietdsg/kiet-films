using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface ITMDBService
    {
        Task<SearchResult<MovieDto>> SearchMoviesAsync(string query);
        Task<MovieDto> GetMovieDetailsAsync(int movieId);
        Task<SearchResult<TVShowDto>> SearchTVShowsAsync(string query);
        Task<TVShowDto> GetTVShowDetailsAsync(int tvShowId);
    }

    public class TMDBService : ITMDBService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string ApiKey = "5d52c55c60f29b44e377fb7bc2108276";
        private readonly string _bearerToken = string.Empty;

        public TMDBService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _bearerToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1ZDUyYzU1YzYwZjI5YjQ0ZTM3N2ZiN2JjMjEwODI3NiIsIm5iZiI6MTczMDY5MTc0OC4yMzMxNTk1LCJzdWIiOiI2NjhkNmJjYWJmY2I5YWM2OTNhYTQ3YWUiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.PmLozEg2fdpxDQvpirCjU62Yp1Y8dZcCtrsTKXbnCps";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        public async Task<SearchResult<MovieDto>> SearchMoviesAsync(string query)
        {
            var url = $"{BaseUrl}/search/movie?query={query}&include_adult=false&language=en-US&page=1";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SearchResult<MovieDto>>() ?? new SearchResult<MovieDto>();
        }

        public async Task<MovieDto> GetMovieDetailsAsync(int movieId)
        {
            var url = $"{BaseUrl}/movie/{movieId}?language=en-US";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MovieDto>() ?? new MovieDto();
        }

        // TV Shows
        public async Task<SearchResult<TVShowDto>> SearchTVShowsAsync(string query)
        {
            var url = $"{BaseUrl}/search/tv?query={query}&include_adult=false&language=en-US&page=1";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SearchResult<TVShowDto>>() ?? new SearchResult<TVShowDto>();
        }

        public async Task<TVShowDto> GetTVShowDetailsAsync(int tvShowId)
        {
            var url = $"{BaseUrl}/tv/{tvShowId}?language=en-US";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TVShowDto>() ?? new TVShowDto();
        }
    }
}