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
}
