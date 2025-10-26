using HackerNewsApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace HackerNewsApi.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetNewestStories_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/stories/newest");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNewestStories_ReturnsValidJsonResponse()
        {
            // Act
            var response = await _client.GetAsync("/api/stories/newest");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(content);
            
            var storiesResponse = JsonSerializer.Deserialize<StoriesResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(storiesResponse);
            Assert.NotNull(storiesResponse.Stories);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        [InlineData(1, 20)]
        public async Task GetNewestStories_WithPagination_ReturnsCorrectStructure(int page, int pageSize)
        {
            // Act
            var response = await _client.GetAsync($"/api/stories/newest?page={page}&pageSize={pageSize}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            
            var storiesResponse = JsonSerializer.Deserialize<StoriesResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(storiesResponse);
            Assert.Equal(page, storiesResponse.Page);
            Assert.Equal(pageSize, storiesResponse.PageSize);
            Assert.True(storiesResponse.Stories.Count() <= pageSize);
        }

        [Fact]
        public async Task GetNewestStories_WithInvalidPage_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/stories/newest?page=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetNewestStories_WithInvalidPageSize_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/stories/newest?pageSize=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}