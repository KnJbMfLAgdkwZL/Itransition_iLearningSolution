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
}