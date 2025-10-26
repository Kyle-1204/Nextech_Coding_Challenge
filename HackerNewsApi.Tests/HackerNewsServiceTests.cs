using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace HackerNewsApi.Tests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<HackerNewsService>> _mockLogger;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<HackerNewsService>>();
            _service = new HackerNewsService(_httpClient, _mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetStoryByIdAsync_WithValidId_ReturnsStory()
        {
            // Arrange
            var storyId = 123;
            var expectedStory = new HackerNewsItem
            {
                Id = storyId,
                Title = "Test Story",
                Url = "https://example.com",
                By = "testuser",
                Time = 1609459200, // Jan 1, 2021
                Score = 100
            };

            var jsonResponse = JsonSerializer.Serialize(expectedStory);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            object? cacheValue = null;
            _mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue))
                     .Returns(false);

            _mockCache.Setup(c => c.CreateEntry(It.IsAny<object>()))
                     .Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _service.GetStoryByIdAsync(storyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storyId, result.Id);
            Assert.Equal("Test Story", result.Title);
            Assert.Equal("https://example.com", result.Url);
        }

        [Fact]
        public async Task GetStoryByIdAsync_WithCachedStory_ReturnsCachedResult()
        {
            // Arrange
            var storyId = 123;
            var cachedStory = new HackerNewsItem
            {
                Id = storyId,
                Title = "Cached Story",
                Url = "https://cached.com"
            };

            object? cacheValue = cachedStory;
            _mockCache.Setup(c => c.TryGetValue($"story_{storyId}", out cacheValue))
                     .Returns(true);

            // Act
            var result = await _service.GetStoryByIdAsync(storyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storyId, result.Id);
            Assert.Equal("Cached Story", result.Title);
            
            // Verify HTTP client was not called
            _mockHttpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync",
                    Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetStoryByIdAsync_WithHttpException_ReturnsNull()
        {
            // Arrange
            var storyId = 123;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            object? cacheValue = null;
            _mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue))
                     .Returns(false);

            // Act
            var result = await _service.GetStoryByIdAsync(storyId);

            // Assert
            Assert.Null(result);
        }
    }
}