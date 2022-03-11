using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ProductGroupService : IProductGroupService
{
    private readonly IGeneralRepository<ProductGroup> _reviewRepository;

    public ProductGroupService(IGeneralRepository<ProductGroup> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<bool> Check(int id)
    {
        return await _reviewRepository.GetOneAsync(product => product.Id == id, CancellationToken.None) != null;
    }

    public async Task<List<ProductGroup>> GetAll()
    {
        return await _reviewRepository.GetAllAsync(product => product.Id > 0, CancellationToken.None);
    }
}