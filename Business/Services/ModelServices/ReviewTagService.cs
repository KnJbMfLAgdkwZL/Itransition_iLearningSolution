using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewTagService : IReviewTagService
{
    private readonly IGeneralRepository<ReviewTag> _reviewTagRepository;

    public ReviewTagService(IGeneralRepository<ReviewTag> reviewTagRepository)
    {
        _reviewTagRepository = reviewTagRepository;
    }

    public async Task AddTagToReview(int reviewId, int tagId)
    {
        await _reviewTagRepository.AddIfNotExistAsync(tag => tag.ReviewId == reviewId && tag.TagId == tagId,
            new ReviewTag()
            {
                ReviewId = reviewId,
                TagId = tagId
            }, CancellationToken.None);
    }

    public async Task DeleteTags(int reviewId)
    {
        var tags = await _reviewTagRepository.GetAllAsync(tag => tag.ReviewId == reviewId, CancellationToken.None);
        foreach (var tag in tags)
        {
            await _reviewTagRepository.RemoveAsync(tag, CancellationToken.None);
            /*await _reviewTagRepository.RemoveIfExistAsync(reviewTag =>
                    reviewTag.Id == tag.Id &&
                    reviewTag.ReviewId == tag.ReviewId &&
                    reviewTag.TagId == tag.TagId, CancellationToken.None
            );*/
        }
    }


    public async Task<List<ReviewTag>> GetTagsNames(int reviewId)
    {
        return await _reviewTagRepository.GetAllIncludeAsync(tag => tag.ReviewId == reviewId, t => t.Tag,
            CancellationToken.None);
    }
}