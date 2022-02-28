using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class CommentService : ICommentService
{
    private readonly IGeneralRepository<Comment> _commentRepository;

    public CommentService(IGeneralRepository<Comment> commentRepository)
    {
        _commentRepository = commentRepository;
    }
}