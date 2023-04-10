using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class ReviewService : BaseService<int, Review>, IReviewService
    {
        IReviewRepository _reviewRepository;
        IUserRepository _userRepository;
        IFilmRepository _filmRepository;
        IRatingRepository _ratingRepository;
        private readonly IConfiguration _configuration;

        public ReviewService(IReviewRepository reviewRepository,
            IConfiguration configuration,
            IUserRepository userRepository,
            IFilmRepository filmRepository,
            IRatingRepository ratingRepository) : base(reviewRepository)
        {
            _reviewRepository = reviewRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _filmRepository = filmRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            return await _reviewRepository.GetPopular(pageSize, pageIndex, filter, sort);
        }

        public List<BaseReviewDTO> GetRecent()
        {
            List<BaseReviewDTO> dtos = new List<BaseReviewDTO>();
            List<Review> reviews = _reviewRepository.GetAll().OrderBy(p => p.CreatedDate).Take(6).ToList();
            foreach (Review review in reviews)
            {
                BaseReviewDTO dto = new BaseReviewDTO();

                //user
                UserReviewDTO userDTO = new UserReviewDTO();
                User user = _userRepository.GetByID(review.UserID);
                if (user != null)
                {
                    userDTO.Avatar = user.Avatar ?? "";
                    userDTO.UserID = user.UserID;
                    userDTO.UserName = user.UserName;
                    userDTO.FullName = user.FullName ?? "";
                }
              
                //film
                FilmReviewDTO filmDTO = new FilmReviewDTO();
                Film film = _filmRepository.GetByID(review.FilmID);
                if (film != null)
                {
                    filmDTO.FilmID = film.FilmID;
                    filmDTO.Poster_path = film.Poster_path;
                    filmDTO.Title = film.Title;
                    filmDTO.Release_date = film.Release_date;
                }
                //rating review
                List<Rating> rating = _ratingRepository.GetAll().Where(p => (p.UserID == user.UserID && p.FilmID == film.FilmID)).ToList();
                if (rating.Count > 0)
                {
                    dto.Rate = rating[0].Score;
                }

                dto.ReviewID = review.ReviewID;
                dto.ReviewDate = review.CreatedDate;
                dto.User = userDTO;
                dto.Film = filmDTO;
                dto.Content = review.Content;
                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
