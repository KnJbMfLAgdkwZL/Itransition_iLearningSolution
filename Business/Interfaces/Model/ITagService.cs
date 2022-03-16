using Database.Models;

namespace Business.Interfaces.Model;

public interface ITagService
{
    Task<List<Tag>> GetTopTags();
    Task<List<Tag>> GetTopTags(string search);
    Task<Tag> AddOrIncrement(string name);
    Task<List<Tag>> FullTextSearchQuery(string search);
}