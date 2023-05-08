using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Like;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Notification;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.User;
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
        IUserRepository _userRepository;
        INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;

        public LikeService(IReviewRepository reviewRepository,
            IConfiguration configuration,
            IFilmRepository filmRepository,
            ILikeRepository likeRepository,
            IUserContext userContext,
            IListRepository listRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository) : base(likeRepository)
        {
            _reviewRepository = reviewRepository;
            _configuration = configuration;
            _filmRepository = filmRepository;
            _likeRepository = likeRepository;
            _userContext = userContext;
            _listRepository = listRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
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
                    newLike.ParentID = id;
                    newLike.UserID = userID;

                    _likeRepository.Add(newLike);

                    //update like count
                    _filmRepository.UpdateLikeCount(id, film.LikesCount + 1);
                } else
                {
                    _likeRepository.Delete(like.LikeID);
                    _filmRepository.UpdateLikeCount(id, film.LikesCount - 1);
                }
                
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
                    newLike.ParentID = id;
                    newLike.UserID = userID;

                    _reviewRepository.UpdateLikeCount(id, review.LikesCount + 1);

                    _likeRepository.Add(newLike);

                    //add notification
                    if (!userID.Equals(review.UserID))
                    {
                        Notification noti = new Notification();
                        Film film = _filmRepository.GetByID(review.FilmID);
                        User userReceive = _userRepository.GetByID(review.UserID);
                        noti.ReceiverUserId = review.UserID;
                        noti.SenderUserID = userID;
                        noti.Seen = false;
                        noti.Content = "";
                        if (film != null)
                        {
                            noti.Content = "liked your review of \"" + film.Title + "\"";
                        }
                        noti.CreatedDate = DateTime.Now;
                        noti.ModifiedDate = DateTime.Now;
                        noti.Date = DateTime.Now;
                        noti.Link = "/u/" + userReceive.UserName + "/reviews/" + review.ReviewID;
                        _notificationRepository.Add(noti);
                    }

                } else
                {
                    _likeRepository.Delete(like.LikeID);
                    _reviewRepository.UpdateLikeCount(id, review.LikesCount - 1);
                }
                //update like count
                
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
                    newLike.ParentID = id;
                    newLike.UserID = userID;

                    _likeRepository.Add(newLike);

                    _listRepository.UpdateLikeCount(id, list.LikesCount + 1);
                    //add notification
                    if (!userID.Equals(list.UserID))
                    {
                        Notification noti = new Notification();
                        User userReceive = _userRepository.GetByID(list.UserID);
                        noti.ReceiverUserId = list.UserID;
                        noti.SenderUserID = userID;
                        noti.Seen = false;
                        noti.Content = "liked your list \"" + list.ListName + "\"";
                        noti.CreatedDate = DateTime.Now;
                        noti.ModifiedDate = DateTime.Now;
                        noti.Date = DateTime.Now;
                        noti.Link = "/u/" + userReceive.UserName + "/lists/" + list.ListID;
                        _notificationRepository.Add(noti);
                    }
                }
                else
                {
                    _likeRepository.Delete(like.LikeID);
                    _listRepository.UpdateLikeCount(id, list.LikesCount - 1);
                }

                res = true;
            }

            return res;
        }
    }
}
