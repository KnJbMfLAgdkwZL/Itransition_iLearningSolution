using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class TagService : ITagService
{
    private readonly IGeneralRepository<Tag> _reviewRepository;

    public TagService(IGeneralRepository<Tag> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<Tag>> GetTopTags()
    {
        var r = await _reviewRepository.GetAllAsyncDescending(t =>
                t.Name != String.Empty,
            t =>
                t.Amount,
            CancellationToken.None
        );
        return r.Take(20).ToList();
    }
}