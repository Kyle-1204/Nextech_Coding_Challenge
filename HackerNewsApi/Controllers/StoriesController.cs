using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Mvc;

// Controller - handles API requests
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

        // Returns paginated list of newest stories
        [HttpGet("newest")]
        public async Task<ActionResult<StoriesResponse>> GetNewestStories(
            [FromQuery] int page = 1, // Page Number
            [FromQuery] int pageSize = 20, //Number of stories per page
            [FromQuery] string? search = null) // Filters by title
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

        // REturns a specific story by its ID
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