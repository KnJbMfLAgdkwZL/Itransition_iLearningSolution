using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.ModelServices;

public class TagService : ITagService
{
    private readonly IGeneralRepository<Tag> _tagRepository;

    public TagService(IGeneralRepository<Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<Tag>> GetTopTags()
    {
        var tags = await _tagRepository.GetAllAsyncDescending(
            tag => tag.Name != string.Empty,
            tag => tag.Amount,
            CancellationToken.None
        );
        return tags.Take(50).ToList();
    }

    public async Task<List<Tag>> GetTopTags(string search)
    {
        var tags = await _tagRepository.GetAllAsyncDescending(
            tag => EF.Functions.Like(tag.Name, $"%{search}%"),
            tag => tag.Amount,
            CancellationToken.None
        );
        return tags.Take(5).ToList();
    }

    public async Task<Tag> AddOrIncrement(string name)
    {
        var tagModel = await _tagRepository.GetOneAsync(tag => tag.Name == name, CancellationToken.None);
        if (tagModel == null)
        {
            tagModel = await _tagRepository.AddAsync(new Tag()
            {
                Name = name,
                Amount = 0
            }, CancellationToken.None);
        }
        else
        {
            tagModel.Amount++;
            await _tagRepository.UpdateAsync(tagModel, CancellationToken.None);
        }

        return tagModel;
    }
}