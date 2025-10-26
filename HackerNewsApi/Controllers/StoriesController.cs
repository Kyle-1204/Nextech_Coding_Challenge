using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;
        private readonly ILogger<StoriesController> _logger;

        public StoriesController(IHackerNewsService hackerNewsService, ILogger<StoriesController> logger)
        {
            _hackerNewsService = hackerNewsService;
            _logger = logger;
        }

        /// <summary>
        /// Get the newest stories from Hacker News
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of stories per page (default: 20, max: 50)</param>
        /// <param name="search">Search term to filter stories by title</param>
        /// <returns>Paginated list of newest stories</returns>
        [HttpGet("newest")]
        public async Task<ActionResult<StoriesResponse>> GetNewestStories(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20, 
            [FromQuery] string? search = null)
        {
            try
            {
                // Validate parameters
                if (page < 1)
                {
                    return BadRequest("Page must be greater than 0");
                }

                if (pageSize < 1 || pageSize > 50)
                {
                    return BadRequest("Page size must be between 1 and 50");
                }

                var result = await _hackerNewsService.GetNewestStoriesAsync(page, pageSize, search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting newest stories");
                return StatusCode(500, "An error occurred while fetching stories");
            }
        }

        /// <summary>
        /// Get a specific story by ID
        /// </summary>
        /// <param name="id">Story ID</param>
        /// <returns>Story details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<HackerNewsItem>> GetStoryById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Story ID must be greater than 0");
                }

                var story = await _hackerNewsService.GetStoryByIdAsync(id);
                
                if (story == null)
                {
                    return NotFound($"Story with ID {id} not found");
                }

                return Ok(story);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting story {StoryId}", id);
                return StatusCode(500, "An error occurred while fetching the story");
            }
        }
    }
}