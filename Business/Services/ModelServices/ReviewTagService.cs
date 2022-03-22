using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewTagService : IReviewTagService
{
    private readonly IGeneralRepository<ReviewTag> _reviewTagRepository;

    public ReviewTagService(
        IGeneralRepository<ReviewTag> reviewTagRepository
    )
    {
        _reviewTagRepository = reviewTagRepository;
    }

    public async Task AddTagToReviewAsync(int reviewId, int tagId, CancellationToken token)
    {
        await _reviewTagRepository.AddIfNotExistAsync(tag => tag.ReviewId == reviewId && tag.TagId == tagId,
            new ReviewTag()
            {
                ReviewId = reviewId,
                TagId = tagId
            }, token);
    }

    public async Task DeleteTagsAsync(int reviewId, CancellationToken token)
    {
        var tags = await _reviewTagRepository.GetAllAsync(tag => tag.ReviewId == reviewId, token);
        foreach (var tag in tags)
        {
            await _reviewTagRepository.RemoveAsync(tag, token);
        }
    }

    public async Task<List<ReviewTag>> GetTagsNamesAsync(int reviewId, CancellationToken token)
    {
        return await _reviewTagRepository.GetAllIncludeAsync(tag => tag.ReviewId == reviewId, t => t.Tag, token);
    }

    public async Task<List<ReviewTag>> GetAllReviewsAsync(int tagId, CancellationToken token)
    {
        return await _reviewTagRepository.GetAllIncludeAsync(tag => tag.TagId == tagId, t => t.Review, token);
    }
}