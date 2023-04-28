using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Rating;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Exceptions;
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
        ILikeRepository _likeRepository;
        ICommentRepository _commentRepository;
        private readonly IConfiguration _configuration;

        public ReviewService(IReviewRepository reviewRepository,
            IConfiguration configuration,
            IUserRepository userRepository,
            IFilmRepository filmRepository,
            IRatingRepository ratingRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository) : base(reviewRepository)
        {
            _reviewRepository = reviewRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _filmRepository = filmRepository;
            _ratingRepository = ratingRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
        }

        public async Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort)
        {
            return await _reviewRepository.GetPopular(pageSize, pageIndex, filter, sort);
        }

        public List<BaseReviewDTO> GetRecent()
        {
            List<BaseReviewDTO> dtos = new List<BaseReviewDTO>();
            List<ListPopularWeekDTO> popularWeek = _reviewRepository.GetRecentWeek();
            List<int> ids = popularWeek.Select(p => p.ListID).ToList();
            List<Review> reviews = _reviewRepository.GetAll().Where(p => ids.Contains(p.ReviewID)).ToList();
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
                BaseFilmDTO filmDTO = new BaseFilmDTO();
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
                dto.CreatedDate = review.CreatedDate;
                dto.User = userDTO;
                dto.Film = filmDTO;
                dto.Content = review.Content;
                dto.WatchedDate = review.WatchedDate;
                dtos.Add(dto);
            }

            return dtos;
        }

        public BaseReviewDTO GetDetail(int id, int limitUser)
        {
            Review review = _reviewRepository.GetByID(id);
            if (review == null)
            {
                throw new ServiceException("Không tìm thấy reivew tương ứng");
            }
            BaseReviewDTO res = new BaseReviewDTO();
            BaseFilmDTO filmDTO = new BaseFilmDTO();
            UserReviewDTO userDTO = new UserReviewDTO();
            List<UserReviewDTO> reviewsLikedByUser = new List<UserReviewDTO>();

            Film film = _filmRepository.GetByID(review.FilmID);
            if (film != null)
            {
                filmDTO.Poster_path = film.Poster_path;
                filmDTO.FilmID = film.FilmID;
                filmDTO.Title = film.Title;
                filmDTO.Release_date = film.Release_date;
            }

            User user = _userRepository.GetByID(review.UserID);
            if (user != null)
            {
                userDTO.Avatar = user.Avatar;
                userDTO.UserName = user.UserName;
                userDTO.UserID = user.UserID;
                userDTO.FullName = user.FullName;
            }

            //user like review
            List<Review> reviewss = _reviewRepository.GetAll().Where(p => p.FilmID == review.FilmID && p.UserID != review.UserID).ToList();
            List<int> parrentIDS = reviewss.Select(p => p.ReviewID).ToList();
            List<Like> likes = _likeRepository.GetAll().Where(p => "Review".Equals(p.Type) && p.UserID == review.UserID && parrentIDS.Contains(p.ParentID)).Take(limitUser).ToList();
            List<int> reviewIDS = likes.Select(x => x.ParentID).ToList();
            List<Review> review2 = _reviewRepository.GetAll().Where(p => reviewIDS.Contains(p.ReviewID)).ToList();
            
            foreach(var rv in review2) {
                User userLike = _userRepository.GetByID(rv.UserID);
                UserReviewDTO dto = new UserReviewDTO();
                dto.Avatar = userLike.Avatar;
                dto.UserName = userLike.UserName;
                dto.UserID = userLike.UserID;
                dto.FullName = userLike.FullName;
                dto.ReviewID = rv.ReviewID;
                reviewsLikedByUser.Add(dto);
            }

            res.Content = review.Content;
            res.ReviewID = review.ReviewID;
            res.CreatedDate = review.CreatedDate;
            res.WatchedDate = review.WatchedDate;
            res.ModifiedDate = review.ModifiedDate;
            res.Rate = review.Score;
            res.TotalLike = review.LikesCount;
            res.Film = filmDTO;
            res.User = userDTO;
            res.ReviewsLikedByUser = reviewsLikedByUser;

            return res;
        }

        public PagingCommentResult GetCommentReview(int reviewID, PagingParameter paging)
        {
            PagingCommentResult res = new PagingCommentResult();
            Review review = _reviewRepository.GetByID(reviewID);
            if (review == null)
            {
                throw new ServiceException("Không tìm thấy review phù hợp");
            }

            var comments = _commentRepository.GetAll().Where(p => p.ParentID == reviewID && "Review".Equals(p.Type.ToString()));
            //var comments = _commentRepository.GetAll();

            int totalCount = comments.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / paging.pageSize);
            comments = comments.Skip((paging.pageIndex - 1) * paging.pageSize).Take(paging.pageSize);
            comments = comments.OrderByDescending(p => p.CreatedDate).ToList();
            List<BaseCommentDTO> commentDTOs = new List<BaseCommentDTO>();
            foreach (Comment comment in comments)
            {
                BaseCommentDTO commentDTO = new BaseCommentDTO();
                User user = _userRepository.GetByID(comment.UserID);

                commentDTO.UserID = comment.UserID;
                commentDTO.Avatar = user.Avatar;
                commentDTO.Username = user.UserName;
                commentDTO.Fullname = user.FullName;
                commentDTO.CommentID = comment.CommentID;
                commentDTO.Content = comment.Content;
                commentDTO.CreatedDate = comment.CreatedDate;

                commentDTOs.Add(commentDTO);

            }
            res.Data = commentDTOs;
            res.TotalPage = totalPages;
            res.Total = totalCount;
            res.PageSize = paging.pageSize;
            res.PageIndex = paging.pageIndex;
            return res;
        }
    }
}
