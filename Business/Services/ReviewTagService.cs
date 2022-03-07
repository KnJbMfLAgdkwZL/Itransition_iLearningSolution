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

    public async Task AddTagToReview(int reviewId, int tagId)
    {
        await _reviewTagRepository.AddIfNotExistAsync(t => t.ReviewId == reviewId && t.TagId == tagId,
            new ReviewTag()
            {
                ReviewId = reviewId,
                TagId = tagId
            }, CancellationToken.None);
    }
}