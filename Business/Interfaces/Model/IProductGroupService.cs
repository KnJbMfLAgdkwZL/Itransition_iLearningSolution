using Database.Models;

namespace Business.Interfaces.Model;

public interface IProductGroupService
{
    Task<bool> Check(int id);
    Task<List<ProductGroup>> GetAll();
    Task<List<ProductGroup>> FullTextSearchQuery(string search);
}