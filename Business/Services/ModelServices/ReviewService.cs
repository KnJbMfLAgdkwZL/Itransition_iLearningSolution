using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Business.Dto.Frontend.FromForm;
using Business.Interfaces.Model;
using Business.Interfaces.Tools;
using DataAccess.Interfaces;
using Database.Models;
using HtmlAgilityPack;

namespace Business.Services.ModelServices;

public class ReviewService : IReviewService
{
    private readonly IGeneralRepository<Review> _reviewRepository;
    private readonly IClearHtmlService _clearHtmlService;

    public ReviewService(
        IGeneralRepository<Review> reviewRepository,
        IClearHtmlService clearHtmlService
    )
    {
        _reviewRepository = reviewRepository;
        _clearHtmlService = clearHtmlService;
    }

    public string? GetImage(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var img = document.DocumentNode.Descendants("img")
            .Where(e =>
            {
                var src = e.GetAttributeValue("src", null) ?? "";
                return !string.IsNullOrEmpty(src);
            }).FirstOrDefault();

        return img?.GetAttributeValue("src", null);
    }

    public string RemoveHtmlTags(string html)
    {
        var str = _clearHtmlService.UnHtml(html);

        var document = new HtmlDocument();
        document.LoadHtml(str);
        str = HtmlEntity.DeEntitize(document.DocumentNode.InnerText);

        str = Regex.Replace(str, "<.*?>", " ");
        str = Regex.Replace(str, @"&nbsp;", " ");
        str = Regex.Replace(str, @"\s{2,}", " ");
        str = Regex.Replace(str, @"[^\w;&#@.:/\\?=|%!() -]", " ");

        str = str.Replace("  ", " ");
        str = str.Trim();

        return str;
    }

    public string CropStr(string str, int size)
    {
        var words = str.Split(" ");
        var stringBuilder = new StringBuilder();
        foreach (var word in words)
        {
            stringBuilder.Append($" {word}");
            if (stringBuilder.Length >= size)
            {
                break;
            }
        }

        return stringBuilder.ToString().Trim();
    }

    public async Task<List<Review>> GetNewReviewsAsync(CancellationToken token)
    {
        var includes = new List<Expression<Func<Review, object>>>()
        {
            review => review.Author,
            review => review.Product,
            review => review.Status,
            review => review.ReviewLike,
            //review => review.ReviewTag,
            review => review.ReviewUserRating
        };

        var reviews = await _reviewRepository.GetAllIncludeManyDescendingAsync(
            review => review.Status.Name != "Deleted",
            includes,
            review => review.CreationDate,
            token);

        return reviews.Take(10).ToList();
    }

    public Dictionary<int, string> GetReviewsImage(List<Review> reviews)
    {
        var reviewsImage = new Dictionary<int, string>();
        foreach (var review in reviews)
        {
            var src = GetImage(review.Content);
            if (src != null)
            {
                reviewsImage.Add(review.Id, src);
            }
        }

        return reviewsImage;
    }

    public void CLearContent(List<Review> reviews)
    {
        foreach (var review in reviews)
        {
            var str = RemoveHtmlTags(review.Content);
            review.Content = CropStr(str, 1000);
        }
    }

    public async Task<List<Review>> GetTopReviewsAsync(CancellationToken token)
    {
        var includes = new List<Expression<Func<Review, object>>>()
        {
            review => review.Author,
            review => review.Product,
            review => review.Status,
            review => review.ReviewLike,
            //review => review.ReviewTag,
            review => review.ReviewUserRating
        };

        var reviews = await _reviewRepository.GetAllIncludeManyDescendingAsync(
            review => review.Status.Name != "Deleted",
            includes,
            review => review.ReviewUserRating.Sum(reviewUserRating => reviewUserRating.Assessment),
            token);

        return reviews.Take(10).ToList();
    }

    public async Task<List<Review>> GetReviewsByIdAsync(List<int> reviewsId, CancellationToken token)
    {
        var includes = new List<Expression<Func<Review, object>>>()
        {
            review => review.Author,
            review => review.Product,
            review => review.Status,
            review => review.ReviewLike,
            //review => review.ReviewTag,
            review => review.ReviewUserRating
        };

        var reviews = await _reviewRepository.GetAllIncludeManyDescendingAsync(
            review => review.Status.Name != "Deleted" &&
                      reviewsId.Contains(review.Id),
            includes,
            review => review.ReviewUserRating.Sum(reviewUserRating => reviewUserRating.Assessment),
            token);

        return reviews.Take(10).ToList();
    }

    public async Task<Review?> CreateAsync(ReviewForm reviewForm, CancellationToken token)
    {
        var review = new Review()
        {
            ProductName = reviewForm.ProductName,
            Title = reviewForm.Title,
            Content = reviewForm.Content,
            AuthorAssessment = reviewForm.AuthorAssessment,
            AverageUserRating = reviewForm.AuthorAssessment,
            ProductId = reviewForm.ProductId,
            AuthorId = reviewForm.AuthorId,
            StatusId = reviewForm.StatusReviewId,
            CreationDate = DateTime.Now,
            RedactionDate = DateTime.Now,
            DeletionDate = DateTime.Now,
        };

        return await _reviewRepository.AddAsync(review, token);
    }

    public async Task<Review?> UpdateAsync(ReviewForm reviewForm, Review review, CancellationToken token)
    {
        review.ProductName = reviewForm.ProductName;
        review.Title = reviewForm.Title;
        review.Content = reviewForm.Content;
        review.AuthorAssessment = reviewForm.AuthorAssessment;
        review.ProductId = reviewForm.ProductId;
        review.StatusId = reviewForm.StatusReviewId;
        review.RedactionDate = DateTime.Now;

        return await _reviewRepository.UpdateAsync(review, token);
    }

    public async Task<Review> UpdateAsync(Review review, CancellationToken token)
    {
        return await _reviewRepository.UpdateAsync(review, token);
    }

    public async Task<Review?> GetOneIncludesAsync(int id, CancellationToken token)
    {
        var includes = new List<Expression<Func<Review, object>>>()
        {
            review => review.Author,
            review => review.Product,
            review => review.Status,
            review => review.Comment,
            review => review.ReviewLike,
            review => review.ReviewTag,
            review => review.ReviewUserRating
        };

        return await _reviewRepository.GetOneIncludeManyAsync(review => review.Id == id, includes, token);
    }

    public async Task<Review?> GetOneAsync(int id, CancellationToken token)
    {
        return await _reviewRepository.GetOneAsync(review => review.Id == id, token);
    }

    public async Task<List<Review>> GetByProductIdAsync(int productId, int takeNum, CancellationToken token)
    {
        var reviews = await _reviewRepository.GetAllAsync(review => review.ProductId == productId, token);

        var orderedEnumerable = reviews.OrderBy(review => review.AverageUserRating);

        return orderedEnumerable.Take(takeNum).ToList();
    }

    public async Task<List<Review>> GetAllIncludesAsync(int userId, CancellationToken token)
    {
        var includes = new List<Expression<Func<Review, object>>>()
        {
            review => review.Author,
            review => review.Product,
            review => review.Status,
            review => review.Comment,
            review => review.ReviewLike,
            review => review.ReviewTag,
            review => review.ReviewUserRating
        };

        return await _reviewRepository.GetAllIncludeManyDescendingAsync(review => review.AuthorId == userId, includes,
            review => review.CreationDate, token);
    }

    public async Task UpdateAverageUserRatingAsync(int id, float averageUserRating, CancellationToken token)
    {
        var review = await GetOneAsync(id, token);
        if (review != null)
        {
            review.AverageUserRating = averageUserRating;
            await _reviewRepository.UpdateAsync(review, token);
        }
    }

    public async Task<List<Review>> FullTextSearchQueryAsync(string search, CancellationToken token)
    {
        return await _reviewRepository.FullTextSearchQueryAsync(search, token);
    }
}