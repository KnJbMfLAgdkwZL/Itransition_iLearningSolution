using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class ReviewLikeService : IReviewLikeService
{
    private readonly IGeneralRepository<ReviewLike> _reviewLikeRepository;

    public ReviewLikeService(IGeneralRepository<ReviewLike> reviewLikeRepository)
    {
        _reviewLikeRepository = reviewLikeRepository;
    }
}
