using HackerNewsApi.Models;

namespace HackerNewsApi.Services
{
    public interface IHackerNewsService
    {
        Task<StoriesResponse> GetNewestStoriesAsync(int page = 1, int pageSize = 20, string? searchTerm = null);
        Task<HackerNewsItem?> GetStoryByIdAsync(int id);
    }
}