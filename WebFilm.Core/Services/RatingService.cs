using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class RatingService : BaseService<int, Rating>, IRatingService
    {
        IRatingRepository _ratingRepository;
        private readonly IConfiguration _configuration;

        public RatingService(IRatingRepository ratingRepository, IConfiguration configuration) : base(ratingRepository)
        {
            _ratingRepository = ratingRepository;
            _configuration = configuration;
        }

        
    }
}
