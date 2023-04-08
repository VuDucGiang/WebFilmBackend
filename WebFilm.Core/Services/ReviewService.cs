using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class ReviewService : BaseService<int, Review>, IReviewService
    {
        IReviewRepository _reviewRepository;
        private readonly IConfiguration _configuration;

        public ReviewService(IReviewRepository reviewRepository, IConfiguration configuration) : base(reviewRepository)
        {
            _reviewRepository = reviewRepository;
            _configuration = configuration;
        }

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            return await _reviewRepository.GetPopular(pageSize, pageIndex, filter, sort);
        }
    }
}
