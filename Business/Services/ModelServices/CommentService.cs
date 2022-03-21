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

    public async Task<List<Comment>> FullTextSearchQuery(string search)
    {
        return await _commentRepository.FullTextSearchQueryAsync(search, CancellationToken.None);
    }
}