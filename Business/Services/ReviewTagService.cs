using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class ReviewTagService : IReviewTagService
{
    private readonly IGeneralRepository<ReviewTag> _reviewTagRepository;

    public ReviewTagService(IGeneralRepository<ReviewTag> reviewTagRepository)
    {
        _reviewTagRepository = reviewTagRepository;
    }
}
