using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class StatusReviewService : IStatusReviewService
{
    private readonly IGeneralRepository<StatusReview> _statusReviewRepository;

    public StatusReviewService(
        IGeneralRepository<StatusReview> statusReviewRepository
    )
    {
        _statusReviewRepository = statusReviewRepository;
    }

    public async Task<StatusReview?> Get(string name)
    {
        return await _statusReviewRepository.GetOneAsync(review => review.Name == name, CancellationToken.None);
    }

    public async Task<List<StatusReview>> GetAll()
    {
        return await _statusReviewRepository.GetAllAsync(review => review.Id > 0, CancellationToken.None);
    }

    public async Task<bool> Check(int id)
    {
        return await _statusReviewRepository.GetOneAsync(review => review.Id == id, CancellationToken.None) != null;
    }
}