using HackerNewsAPI.Interfaces;
using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HackerNewsAPI.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsService> _logger;
        private readonly HackerNewsSettings _settings;

        public HackerNewsService(
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<HackerNewsService> logger,
            IOptions<HackerNewsSettings> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IList<HackerNewsItem>> GetNewestStoriesAsync()
        {
            if (_cache.TryGetValue(_settings.CacheKey, out List<HackerNewsItem>? cachedStories) && cachedStories?.Any() == true)
                return cachedStories;

            var ids = await FetchStoryIdsAsync();
            if (ids.Count == 0)
                return [];

            var storyTasks = ids
                .Take(_settings.MaxStoriesToFetch)
                .Select(FetchStoryByIdAsync);

            var fetchedStories = new List<HackerNewsItem>();

            foreach (var chunk in storyTasks.Chunk(_settings.BatchSize))
            {
                var completed = await Task.WhenAll(chunk);
                fetchedStories.AddRange(completed);
            }

            var validStories = fetchedStories
                .Where(story => !string.IsNullOrWhiteSpace(story?.Title) && !string.IsNullOrWhiteSpace(story?.Url))
                .ToList();

            _cache.Set(_settings.CacheKey, validStories, TimeSpan.FromMinutes(_settings.CacheTimeSpan));
            return validStories;
        }

        private async Task<List<int>> FetchStoryIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(_settings.NewStoriesUrl);
                return string.IsNullOrWhiteSpace(response)
                    ? []
                    : JsonConvert.DeserializeObject<List<int>>(response) ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new story IDs from Hacker News.");
                return [];
            }
        }

        private async Task<HackerNewsItem> FetchStoryByIdAsync(int id)
        {
            var itemUrl = string.Format(_settings.ItemUrlTemplate, id);
            try
            {
                var response = await _httpClient.GetStringAsync(itemUrl);
                return string.IsNullOrWhiteSpace(response)
                    ? new()
                    : JsonConvert.DeserializeObject<HackerNewsItem>(response) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch or deserialize story with ID {Id}", id);
                return new();
            }
        }
    }
}
