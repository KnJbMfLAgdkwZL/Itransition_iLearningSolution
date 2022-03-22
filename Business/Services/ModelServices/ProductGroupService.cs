using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ProductGroupService : IProductGroupService
{
    private readonly IGeneralRepository<ProductGroup> _productGroupRepository;

    public ProductGroupService(
        IGeneralRepository<ProductGroup> reviewRepository
    )
    {
        _productGroupRepository = reviewRepository;
    }

    public async Task<bool> CheckAsync(int id, CancellationToken token)
    {
        return await _productGroupRepository.GetOneAsync(product => product.Id == id, token) != null;
    }

    public async Task<List<ProductGroup>> GetAllAsync(CancellationToken token)
    {
        return await _productGroupRepository.GetAllAsync(product => product.Id > 0, token);
    }

    public async Task<List<ProductGroup>> FullTextSearchQueryAsync(string search, CancellationToken token)
    {
        return await _productGroupRepository.FullTextSearchQueryAsync(search, token);
    }
}