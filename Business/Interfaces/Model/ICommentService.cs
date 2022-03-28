using Database.Models;

namespace Business.Interfaces.Model;

public interface ICommentService
{
    Task<List<Comment>> FullTextSearchQueryAsync(string search, CancellationToken token);
    Task<List<Comment>> GetComments(int reviewId, CancellationToken token);
    Task<Comment> CreateComment(int reviewId, int userId, string content, CancellationToken token);
}