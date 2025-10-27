using HackerNewsApi.Models;

namespace HackerNewsApi.Services
{
    // Interface for Hacker News service
    public interface IHackerNewsService
    {
        Task<StoriesResponse> GetNewestStoriesAsync(int page = 1, int pageSize = 20, string? searchTerm = null);
        Task<HackerNewsItem?> GetStoryByIdAsync(int id);
    }
}