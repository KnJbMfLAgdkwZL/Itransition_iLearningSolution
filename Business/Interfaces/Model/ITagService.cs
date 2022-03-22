using Database.Models;

namespace Business.Interfaces.Model;

public interface ITagService
{
    Task<List<Tag>> GetTopTagsAsync(CancellationToken token);
    Task<List<Tag>> GetTopTagsAsync(string search, CancellationToken token);
    Task<Tag> AddOrIncrementAsync(string name, CancellationToken token);
    Task<List<Tag>> FullTextSearchQueryAsync(string search, CancellationToken token);
}