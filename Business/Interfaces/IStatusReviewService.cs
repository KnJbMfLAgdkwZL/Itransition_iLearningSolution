using Database.Models;

namespace Business.Interfaces;

public interface IStatusReviewService
{
    Task<StatusReview?> Get(string name);
    Task<List<StatusReview>> GetAll();
    Task<bool> Check(int id);
}