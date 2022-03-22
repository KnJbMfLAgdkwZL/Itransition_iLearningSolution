using Database.Models;

namespace Business.Interfaces.Model;

public interface IProductGroupService
{
    Task<bool> CheckAsync(int id, CancellationToken token);
    Task<List<ProductGroup>> GetAllAsync(CancellationToken token);
    Task<List<ProductGroup>> FullTextSearchQueryAsync(string search, CancellationToken token);
}