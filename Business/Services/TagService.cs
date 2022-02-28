using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class TagService : ITagService
{
    private readonly IGeneralRepository<Tag> _tagRepository;

    public TagService(IGeneralRepository<Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<Tag>> GetTopTags()
    {
        var r = await _tagRepository.GetAllAsyncDescending(t =>
                t.Name != String.Empty,
            t =>
                t.Amount,
            CancellationToken.None
        );
        return r.Take(20).ToList();
    }
}