using Database.Models;

namespace Business.Interfaces.Model;

public interface ICommentService
{
    Task<List<Comment>> FullTextSearchQueryAsync(string search, CancellationToken token);
}