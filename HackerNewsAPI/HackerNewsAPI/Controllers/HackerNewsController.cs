using HackerNewsAPI.Interfaces;
using HackerNewsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;
        private readonly ILogger<HackerNewsController> _logger;

        public HackerNewsController(IHackerNewsService hackerNewsService, ILogger<HackerNewsController> logger)
        {
            _hackerNewsService = hackerNewsService ?? throw new ArgumentNullException(nameof(hackerNewsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("newest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNewestAsync()
        {
            try
            {
                var stories = await _hackerNewsService.GetNewestStoriesAsync();
                return Ok(stories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch stories from Hacker News.");
                return StatusCode(500, "An error occurred while fetching data.");
            }
        }
    }
}
