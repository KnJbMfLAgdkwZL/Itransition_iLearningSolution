using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class ProductGroupService : IProductGroupService
{
    private readonly IGeneralRepository<ProductGroup> _reviewRepository;

    public ProductGroupService(IGeneralRepository<ProductGroup> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<bool> Check(int productId)
    {
        return await _reviewRepository
            .GetOneAsync(group => group.Id == productId, CancellationToken.None) != null;
    }

    public async Task<List<ProductGroup>> GetAll()
    {
        return await _reviewRepository
            .GetAllAsync(group => group.Id > 0, CancellationToken.None);
    }
}