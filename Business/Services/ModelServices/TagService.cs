using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.ModelServices;

public class TagService : ITagService
{
    private readonly IGeneralRepository<Tag> _tagRepository;

    public TagService(
        IGeneralRepository<Tag> tagRepository
    )
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<Tag>> GetTopTagsAsync(CancellationToken token)
    {
        var tags = await _tagRepository.GetAllDescendingAsync(
            tag => tag.Name != string.Empty,
            tag => tag.Amount,
            token);
        
        return tags.Take(50).ToList();
    }

    public async Task<List<Tag>> GetTopTagsAsync(string search, CancellationToken token)
    {
        var tags = await _tagRepository.GetAllDescendingAsync(
            tag => EF.Functions.Like(tag.Name, $"%{search}%"),
            tag => tag.Amount,
            token);
        
        return tags.Take(5).ToList();
    }

    public async Task<Tag> AddOrIncrementAsync(string name, CancellationToken token)
    {
        var tagModel = await _tagRepository.GetOneAsync(tag => tag.Name == name, token);
        if (tagModel == null)
        {
            tagModel = await _tagRepository.AddAsync(new Tag()
            {
                Name = name,
                Amount = 1
            }, token);
        }
        else
        {
            tagModel.Amount++;
            await _tagRepository.UpdateAsync(tagModel, token);
        }

        return tagModel;
    }

    public async Task<List<Tag>> FullTextSearchQueryAsync(string search, CancellationToken token)
    {
        return await _tagRepository.FullTextSearchQueryAsync(search, token);
    }
}