using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

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
                t.Name != string.Empty,
            t =>
                t.Amount,
            CancellationToken.None
        );
        return r.Take(50).ToList();
    }

    public async Task<List<Tag>> GetTopTags(string search)
    {
        var r = await _tagRepository.GetAllAsyncDescending(t =>
                EF.Functions.Like(t.Name, $"%{search}%"),
            t => t.Amount,
            CancellationToken.None
        );
        return r.Take(5).ToList();
    }

    public async Task<Tag> AddOrIncrement(string name)
    {
        var tag = await _tagRepository.GetOneAsync(t => t.Name == name, CancellationToken.None);
        if (tag != null)
        {
            tag.Amount++;
            await _tagRepository.UpdateAsync(tag, CancellationToken.None);
        }
        else
        {
            tag = await _tagRepository.AddAsync(new Tag()
            {
                Name = name,
                Amount = 1
            }, CancellationToken.None);
        }

        return tag;
    }
}