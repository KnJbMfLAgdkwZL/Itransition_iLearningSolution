using Database.Models;

namespace Business.Interfaces;

public interface IProductGroupService
{
    Task<bool> Check(int productId);
    Task<List<ProductGroup>> GetAll();
}