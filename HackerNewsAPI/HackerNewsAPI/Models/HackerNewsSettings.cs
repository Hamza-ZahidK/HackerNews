namespace HackerNewsAPI.Models
{
    public class HackerNewsSettings
    {
        public required string NewStoriesUrl { get; set; }
        public required string ItemUrlTemplate { get; set; }
        public required string CacheKey { get; set; }
        public int MaxStoriesToFetch { get; set; }
        public int BatchSize { get; set; }
        public int CacheTimeSpan { get; set; }
    }
}
