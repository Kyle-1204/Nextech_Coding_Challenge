using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace HackerNewsApi.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsService> _logger;
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";
        private const string NewestStoriesCacheKey = "newest_stories";
        private const int CacheExpirationMinutes = 10;

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache, ILogger<HackerNewsService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<StoriesResponse> GetNewestStoriesAsync(int page = 1, int pageSize = 20, string? searchTerm = null)
        {
            try
            {
                var storyIds = await GetNewestStoryIdsAsync();
                
                // Apply search filter if provided
                var filteredStories = new List<HackerNewsItem>();
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    // For search, we need to get all stories first, then filter
                    var allStories = await GetStoriesByIdsAsync(storyIds.Take(500)); // Limit to first 500 for performance
                    filteredStories = allStories
                        .Where(story => story.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                }
                else
                {
                    // For pagination without search, get only the stories for the current page
                    var pageStoryIds = storyIds.Skip((page - 1) * pageSize).Take(pageSize);
                    filteredStories = (await GetStoriesByIdsAsync(pageStoryIds)).ToList();
                }

                var totalCount = string.IsNullOrEmpty(searchTerm) ? storyIds.Count() : filteredStories.Count;
                
                // Apply pagination to filtered results
                var pagedStories = string.IsNullOrEmpty(searchTerm) 
                    ? filteredStories 
                    : filteredStories.Skip((page - 1) * pageSize).Take(pageSize);

                return new StoriesResponse
                {
                    Stories = pagedStories,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching newest stories");
                throw;
            }
        }

        public async Task<HackerNewsItem?> GetStoryByIdAsync(int id)
        {
            try
            {
                var cacheKey = $"story_{id}";
                
                if (_cache.TryGetValue(cacheKey, out HackerNewsItem? cachedStory))
                {
                    return cachedStory;
                }

                var response = await _httpClient.GetStringAsync($"{BaseUrl}/item/{id}.json");
                var story = JsonSerializer.Deserialize<HackerNewsItem>(response);

                if (story != null)
                {
                    _cache.Set(cacheKey, story, TimeSpan.FromMinutes(CacheExpirationMinutes));
                }

                return story;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching story {StoryId}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching story {StoryId}", id);
                throw;
            }
        }

        private async Task<IEnumerable<int>> GetNewestStoryIdsAsync()
        {
            if (_cache.TryGetValue(NewestStoriesCacheKey, out IEnumerable<int>? cachedIds))
            {
                return cachedIds!;
            }

            var response = await _httpClient.GetStringAsync($"{BaseUrl}/newstories.json");
            var storyIds = JsonSerializer.Deserialize<int[]>(response) ?? Array.Empty<int>();

            _cache.Set(NewestStoriesCacheKey, storyIds, TimeSpan.FromMinutes(CacheExpirationMinutes));

            return storyIds;
        }

        private async Task<IEnumerable<HackerNewsItem>> GetStoriesByIdsAsync(IEnumerable<int> storyIds)
        {
            var tasks = storyIds.Select(GetStoryByIdAsync);
            var stories = await Task.WhenAll(tasks);
            
            // Filter out null stories and stories without titles
            return stories
                .Where(story => story != null && !string.IsNullOrEmpty(story.Title))
                .Cast<HackerNewsItem>();
        }
    }
}