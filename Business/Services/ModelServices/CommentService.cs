using System.Linq.Expressions;
using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class CommentService : ICommentService
{
    private readonly IGeneralRepository<Comment> _commentRepository;

    public CommentService(
        IGeneralRepository<Comment> commentRepository
    )
    {
        _commentRepository = commentRepository;
    }

    public async Task<List<Comment>> FullTextSearchQueryAsync(string search, CancellationToken token)
    {
        return await _commentRepository.FullTextSearchQueryAsync(search, token);
    }

    public async Task<List<Comment>> GetComments(int reviewId, CancellationToken token)
    {
        var includes = new List<Expression<Func<Comment, object>>>()
        {
            comment => comment.User
        };
        var comments = await _commentRepository.GetAllIncludeManyAsync(
            comment => comment.ReviewId == reviewId,
            includes, token);
        return comments.Take(50).ToList();
    }

    public async Task<Comment> CreateComment(int reviewId, int userId, string content, CancellationToken token)
    {
        return await _commentRepository.AddAsync(new Comment()
        {
            ReviewId = reviewId,
            UserId = userId,
            Content = content,
            Date = DateTime.Now
        }, token);
    }
}