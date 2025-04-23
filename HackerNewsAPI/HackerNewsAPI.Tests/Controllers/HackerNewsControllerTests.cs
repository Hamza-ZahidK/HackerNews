using HackerNewsAPI.Controllers;
using HackerNewsAPI.Interfaces;
using HackerNewsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HackerNewsAPI.Tests.Controllers
{
    public class HackerNewsControllerTests
    {
        private readonly Mock<IHackerNewsService> _mockService;
        private readonly Mock<ILogger<HackerNewsController>> _mockLogger;
        private readonly HackerNewsController _controller;

        public HackerNewsControllerTests()
        {
            _mockService = new Mock<IHackerNewsService>();
            _mockLogger = new Mock<ILogger<HackerNewsController>>();
            _controller = new HackerNewsController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetNewestAsync_ReturnsOk_WithStories()
        {
            // Arrange
            var stories = new List<HackerNewsItem> { new() { Id = 1, Title = "Test", Url = "http://test.com" } };
            _mockService.Setup(s => s.GetNewestStoriesAsync()).ReturnsAsync(stories);

            // Act
            var result = await _controller.GetNewestAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<HackerNewsItem>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetNewestAsync_Returns500_OnException()
        {
            // Arrange
            _mockService.Setup(s => s.GetNewestStoriesAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetNewestAsync();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
