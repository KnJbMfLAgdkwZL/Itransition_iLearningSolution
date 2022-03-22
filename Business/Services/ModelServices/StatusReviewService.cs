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

    public async Task<StatusReview?> GetAsync(string name, CancellationToken token)
    {
        return await _statusReviewRepository.GetOneAsync(review => review.Name == name, token);
    }

    public async Task<List<StatusReview>> GetAllAsync(CancellationToken token)
    {
        return await _statusReviewRepository.GetAllAsync(review => review.Id > 0, token);
    }

    public async Task<bool> CheckAsync(int id, CancellationToken token)
    {
        return await _statusReviewRepository.GetOneAsync(review => review.Id == id, token) != null;
    }
}