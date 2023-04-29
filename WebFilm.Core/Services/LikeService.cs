using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class LikeService : BaseService<int, Like>, ILikeService
    {
        IReviewRepository _reviewRepository;
        IUserContext _userContext;
        IFilmRepository _filmRepository;
        ILikeRepository _likeRepository;
        IListRepository _listRepository;
        private readonly IConfiguration _configuration;

        public LikeService(IReviewRepository reviewRepository,
            IConfiguration configuration,
            IFilmRepository filmRepository,
            ILikeRepository likeRepository,
            IUserContext userContext,
            IListRepository listRepository) : base(likeRepository)
        {
            _reviewRepository = reviewRepository;
            _configuration = configuration;
            _filmRepository = filmRepository;
            _likeRepository = likeRepository;
            _userContext = userContext;
            _listRepository = listRepository;
        }

        public bool newLike(int id, string type)
        {
            Guid userID = (Guid)_userContext.UserId;

            if (userID == Guid.Empty)
            {
                throw new ServiceException("Hành động không khả thi");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ServiceException("Type không được null hoặc khoảng trắng");
            }

            bool res = false;
           
            if ("film".Equals(type))
            {
                Film film = _filmRepository.GetByID(id);

                if (film == null)
                {
                    throw new ServiceException("Không tìm thấy film hợp lệ");
                }

                Like like = _likeRepository.getlikeByUserIDAndTypeAndParentID(userID, "Film", id);
                //save like
                if (like == null)
                {
                    Like newLike = new Like();
                    newLike.Type = "Film";
                    newLike.CreatedDate = DateTime.Now;
                    newLike.ModifiedDate = DateTime.Now;
                    newLike.Date = DateTime.Now;
                    newLike.ParentID = id;
                    newLike.UserID = userID;

                    _likeRepository.Add(newLike);
                }
                //update like count
                film.LikesCount = film.LikesCount + 1;
                _filmRepository.Edit(id, film);
                res = true;
            }

            if ("review".Equals(type))
            {
                Review review = _reviewRepository.GetByID(id);

                if (review == null)
                {
                    throw new ServiceException("Không tìm thấy review hợp lệ");
                }

                Like like = _likeRepository.getlikeByUserIDAndTypeAndParentID(userID, "Review", id);
                //save like
                if (like == null)
                {
                    Like newLike = new Like();
                    newLike.Type = "Review";
                    newLike.CreatedDate = DateTime.Now;
                    newLike.ModifiedDate = DateTime.Now;
                    newLike.Date = DateTime.Now;
                    newLike.ParentID = id;
                    newLike.UserID = userID;

                    _likeRepository.Add(newLike);
                }
                //update like count
                review.LikesCount = review.LikesCount + 1;
                _reviewRepository.Edit(id, review);
                res = true;
            }

            if ("list".Equals(type))
            {
                List list = _listRepository.GetByID(id);

                if (list == null)
                {
                    throw new ServiceException("Không tìm thấy list hợp lệ");
                }

                Like like = _likeRepository.getlikeByUserIDAndTypeAndParentID(userID, "List", id);
                //save like
                if (like == null)
                {
                    Like newLike = new Like();
                    newLike.Type = "List";
                    newLike.CreatedDate = DateTime.Now;
                    newLike.ModifiedDate = DateTime.Now;
                    newLike.Date = DateTime.Now;
                    newLike.ParentID = id;
                    newLike.UserID = userID;

                    _likeRepository.Add(newLike);
                }
                //update like count
                list.LikesCount = list.LikesCount + 1;
                _listRepository.Edit(id, list);
                res = true;
            }

            return res;
        }
    }
}
