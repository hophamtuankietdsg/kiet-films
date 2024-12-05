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
        Task<VideoResponse> GetMovieVideosAsync(int movieId);
        Task<VideoResponse> GetTVShowVideosAsync(int tvShowId);
    }

    public class TMDBService : ITMDBService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private readonly ILogger<TMDBService> _logger;

        public TMDBService(HttpClient httpClient, ILogger<TMDBService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            string bearerToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1ZDUyYzU1YzYwZjI5YjQ0ZTM3N2ZiN2JjMjEwODI3NiIsIm5iZiI6MTczMDY5MTc0OC4yMzMxNTk1LCJzdWIiOiI2NjhkNmJjYWJmY2I5YWM2OTNhYTQ3YWUiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.PmLozEg2fdpxDQvpirCjU62Yp1Y8dZcCtrsTKXbnCps";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        private async Task<T> GetAsync<T>(string url) where T : class, new()
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>() ?? new T();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling TMDB API: {url}");
                throw;
            }
        }

        public async Task<SearchResult<MovieDto>> SearchMoviesAsync(string query)
        {
            var url = $"{BaseUrl}/search/movie?query={query}&include_adult=false&language=en-US&page=1";
            return await GetAsync<SearchResult<MovieDto>>(url);
        }

        public async Task<MovieDto> GetMovieDetailsAsync(int movieId)
        {
            var url = $"{BaseUrl}/movie/{movieId}?language=en-US&append_to_response=genres";
            var movieDetails = await GetAsync<MovieDto>(url);

            // Get genre_ids
            var searchUrl = $"{BaseUrl}/search/movie?query={movieDetails.Title}&include_adult=false&language=en-US&page=1";
            var searchResult = await GetAsync<SearchResult<MovieDto>>(searchUrl);
            
            var movie = searchResult.Results.FirstOrDefault(m => m.Id == movieId);
            if (movie != null)
            {
                movieDetails.GenreIds = movie.GenreIds;
            }

            return movieDetails;
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
            var url = $"{BaseUrl}/tv/{tvShowId}?language=en-US&append_to_response=genres";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var tvShowDetails = await response.Content.ReadFromJsonAsync<TVShowDto>();

            // Lấy genre_ids từ kết quả search
            var searchUrl = $"{BaseUrl}/search/tv?query={tvShowDetails?.Name}&include_adult=false&language=en-US&page=1";
            var searchRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(searchUrl)
            };

            var searchResponse = await _httpClient.SendAsync(searchRequest);
            searchResponse.EnsureSuccessStatusCode();
            var searchResult = await searchResponse.Content.ReadFromJsonAsync<SearchResult<TVShowDto>>();
            
            var tvShow = searchResult?.Results.FirstOrDefault(t => t.Id == tvShowId);
            if (tvShow != null && tvShowDetails != null)
            {
                tvShowDetails.GenreIds = tvShow.GenreIds;
            }

            return tvShowDetails ?? new TVShowDto();
        }

        public async Task<VideoResponse> GetMovieVideosAsync(int movieId)
        {
            var url = $"{BaseUrl}/movie/{movieId}/videos?language=en-US";
            return await GetAsync<VideoResponse>(url);
        }

        public async Task<VideoResponse> GetTVShowVideosAsync(int tvShowId)
        {
            var url = $"{BaseUrl}/tv/{tvShowId}/videos?language=en-US";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VideoResponse>() ?? new VideoResponse();
        }
    }
}