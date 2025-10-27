namespace HackerNewsApi.Models
{
    public class StoriesResponse // Model for paginated stories response
    {
        public IEnumerable<HackerNewsItem> Stories { get; set; } = new List<HackerNewsItem>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}