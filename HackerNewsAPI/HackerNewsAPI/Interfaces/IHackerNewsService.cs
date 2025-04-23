using HackerNewsAPI.Models;

namespace HackerNewsAPI.Interfaces
{
    public interface IHackerNewsService
    {
        Task<IList<HackerNewsItem>> GetNewestStoriesAsync();
    }
}
