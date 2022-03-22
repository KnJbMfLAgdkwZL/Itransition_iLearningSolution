using Database.Models;

namespace Business.Interfaces.Model;

public interface IStatusReviewService
{
    Task<StatusReview?> GetAsync(string name, CancellationToken token);
    Task<List<StatusReview>> GetAllAsync(CancellationToken token);
    Task<bool> CheckAsync(int id, CancellationToken token);
}