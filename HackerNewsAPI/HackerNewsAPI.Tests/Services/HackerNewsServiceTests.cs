using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace HackerNewsAPI.Tests.Services
{
    public class HackerNewsServiceTests
    {
        private readonly HackerNewsService _service;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly MemoryCache _realCache;
        private readonly ILogger<HackerNewsService> _logger;
        private readonly HttpClient _httpClient;

        public HackerNewsServiceTests()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
               {
                   if (request.RequestUri.AbsoluteUri.Contains("newstories"))
                   {
                       return new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content = new StringContent(JsonConvert.SerializeObject(new List<int> { 1, 2 }))
                       };
                   }
                   else
                   {
                       var item = new HackerNewsItem { Id = 1, Title = "Sample", Url = "http://sample.com" };
                       return new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content = new StringContent(JsonConvert.SerializeObject(item))
                       };
                   }
               });

            _httpClient = new HttpClient(handlerMock.Object);

            _mockCache = new Mock<IMemoryCache>();
            _realCache = new MemoryCache(new MemoryCacheOptions());

            var options = Options.Create(new HackerNewsSettings
            {
                NewStoriesUrl = "https://hacker-news.firebaseio.com/v0/newstories.json",
                ItemUrlTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json",
                CacheKey = "hacker-news-cache",
                MaxStoriesToFetch = 2,
                BatchSize = 2
            });

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<HackerNewsService>();
            _service = new HackerNewsService(_httpClient, _realCache, _logger, options);
        }

        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsStories()
        {
            // Act
            var stories = await _service.GetNewestStoriesAsync();

            // Assert
            Assert.NotNull(stories);
            Assert.NotEmpty(stories);
            Assert.All(stories, story => Assert.False(string.IsNullOrWhiteSpace(story.Title)));
        }

        [Fact]
        public async Task GetNewestStoriesAsync_UsesCache()
        {
            // Act
            var firstCall = await _service.GetNewestStoriesAsync();
            var secondCall = await _service.GetNewestStoriesAsync();

            // Should hit the cache on second call
            Assert.Equal(firstCall.Count, secondCall.Count);
        }

        [Fact]
        public async Task FetchStoryIdsAsync_ReturnsEmptyList_OnFailure()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
               .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(handlerMock.Object);

            var options = Options.Create(new HackerNewsSettings
            {
                NewStoriesUrl = "https://fail.com",
                ItemUrlTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json",
                CacheKey = "fail-key",
                MaxStoriesToFetch = 2,
                BatchSize = 2
            });

            var service = new HackerNewsService(httpClient, _realCache, _logger, options);

            var result = await service.GetNewestStoriesAsync();
            Assert.Empty(result);
        }
    }
}
