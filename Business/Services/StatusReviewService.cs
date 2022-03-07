using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class StatusReviewService : IStatusReviewService
{
    private readonly IGeneralRepository<StatusReview> _statusReviewRepository;

    public StatusReviewService(IGeneralRepository<StatusReview> statusReviewRepository)
    {
        _statusReviewRepository = statusReviewRepository;
    }

    public async Task<StatusReview?> Get(string name)
    {
        return await _statusReviewRepository.GetOneAsync(r =>
            r.Name == name, CancellationToken.None);
    }

    public async Task<List<StatusReview>> GetAll()
    {
        return await _statusReviewRepository.GetAllAsync(r => r.Name != "Deleted", CancellationToken.None);
    }

    public async Task<bool> Check(int id)
    {
        return await _statusReviewRepository.GetOneAsync(group => group.Id == id, CancellationToken.None) != null;
    }
}