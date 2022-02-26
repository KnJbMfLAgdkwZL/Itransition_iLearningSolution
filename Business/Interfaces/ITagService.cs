using Database.Models;

namespace Business.Interfaces;

public interface ITagService
{
    Task<List<Tag>> GetTopTags();
}