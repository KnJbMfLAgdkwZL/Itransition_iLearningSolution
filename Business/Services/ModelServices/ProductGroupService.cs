using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ProductGroupService : IProductGroupService
{
    private readonly IGeneralRepository<ProductGroup> _productGroupRepository;

    public ProductGroupService(IGeneralRepository<ProductGroup> reviewRepository)
    {
        _productGroupRepository = reviewRepository;
    }

    public async Task<bool> Check(int id)
    {
        return await _productGroupRepository.GetOneAsync(product => product.Id == id, CancellationToken.None) != null;
    }

    public async Task<List<ProductGroup>> GetAll()
    {
        return await _productGroupRepository.GetAllAsync(product => product.Id > 0, CancellationToken.None);
    }
    public async Task<List<ProductGroup>> FullTextSearchQuery(string search)
    {
        return await _productGroupRepository.FullTextSearchQueryAsync(search, CancellationToken.None);
    }
}