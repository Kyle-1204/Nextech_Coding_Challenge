using HackerNewsApi.Controllers;
using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HackerNewsApi.Tests
{
    public class StoriesControllerTests
    {
        private readonly Mock<IHackerNewsService> _mockHackerNewsService;
        private readonly Mock<ILogger<StoriesController>> _mockLogger;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _mockHackerNewsService = new Mock<IHackerNewsService>();
            _mockLogger = new Mock<ILogger<StoriesController>>();
            _controller = new StoriesController(_mockHackerNewsService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetNewestStories_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var expectedResponse = new StoriesResponse
            {
                Stories = new List<HackerNewsItem>
                {
                    new HackerNewsItem { Id = 1, Title = "Test Story 1", Url = "https://example.com/1" },
                    new HackerNewsItem { Id = 2, Title = "Test Story 2", Url = "https://example.com/2" }
                },
                TotalCount = 2,
                Page = 1,
                PageSize = 20
            };

            _mockHackerNewsService
                .Setup(s => s.GetNewestStoriesAsync(1, 20, null))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetNewestStories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<StoriesResponse>(okResult.Value);
            Assert.Equal(2, response.Stories.Count());
            Assert.Equal(2, response.TotalCount);
        }

        [Fact]
        public async Task GetNewestStories_WithSearchTerm_ReturnsFilteredResults()
        {
            // Arrange
            var expectedResponse = new StoriesResponse
            {
                Stories = new List<HackerNewsItem>
                {
                    new HackerNewsItem { Id = 1, Title = "Angular Tutorial", Url = "https://example.com/1" }
                },
                TotalCount = 1,
                Page = 1,
                PageSize = 20
            };

            _mockHackerNewsService
                .Setup(s => s.GetNewestStoriesAsync(1, 20, "Angular"))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetNewestStories(search: "Angular");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<StoriesResponse>(okResult.Value);
            Assert.Single(response.Stories);
            Assert.Contains("Angular", response.Stories.First().Title);
        }

        [Fact]
        public async Task GetNewestStories_WithInvalidPage_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetNewestStories(page: 0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Page must be greater than 0", badRequestResult.Value);
        }

        [Fact]
        public async Task GetNewestStories_WithInvalidPageSize_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetNewestStories(pageSize: 0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Page size must be between 1 and 50", badRequestResult.Value);
        }

        [Fact]
        public async Task GetStoryById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedStory = new HackerNewsItem 
            { 
                Id = 1, 
                Title = "Test Story", 
                Url = "https://example.com" 
            };

            _mockHackerNewsService
                .Setup(s => s.GetStoryByIdAsync(1))
                .ReturnsAsync(expectedStory);

            // Act
            var result = await _controller.GetStoryById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var story = Assert.IsType<HackerNewsItem>(okResult.Value);
            Assert.Equal(1, story.Id);
            Assert.Equal("Test Story", story.Title);
        }

        [Fact]
        public async Task GetStoryById_WithInvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStoryById(0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Story ID must be greater than 0", badRequestResult.Value);
        }

        [Fact]
        public async Task GetStoryById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            _mockHackerNewsService
                .Setup(s => s.GetStoryByIdAsync(999))
                .ReturnsAsync((HackerNewsItem?)null);

            // Act
            var result = await _controller.GetStoryById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Story with ID 999 not found", notFoundResult.Value);
        }
    }
}
