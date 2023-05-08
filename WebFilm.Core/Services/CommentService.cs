using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Enitites.Notification;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.User.Profile;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class CommentService : BaseService<int, Comment>, ICommentService
    {
        ICommentRepository _commentRepository;
        IListRepository _listRepository;
        IUserRepository _userRepository;
        IUserContext _userContext;
        IReviewRepository _reviewRepository;
        INotificationRepository _notificationRepository;
        IFilmRepository _filmRepository;
        private readonly IConfiguration _configuration;

        public CommentService(ICommentRepository commentRepository, IConfiguration configuration,
            IListRepository listRepository, IUserRepository userRepository, IUserContext userContext, IReviewRepository reviewRepository, INotificationRepository notificationRepository,
            IFilmRepository filmRepository) : base(commentRepository)
        {
            _commentRepository = commentRepository;
            _configuration = configuration;
            _listRepository = listRepository;
            _userRepository = userRepository;
            _userContext = userContext;
            _reviewRepository = reviewRepository;
            _notificationRepository = notificationRepository;
            _filmRepository = filmRepository;
        }

        public int CreateCommentInList(int ListID, CommentCreateDTO dto)
        {
            Guid userID = (Guid)_userContext.UserId;
            User user = _userRepository.GetByID(userID);
            if (user == null)
            {
                throw new ServiceException("Hành động không hợp lệ");
            }

            List list = _listRepository.GetByID(ListID);
            if (list == null)
            {
                throw new ServiceException("Không tìm thấy list phù hợp");
            }
            try
            {
                int commentCount = list.CommentsCount + 1;

                Comment newComment = new Comment();
                newComment.Content = dto.Content;
                newComment.ParentID = ListID;
                newComment.UserID = (Guid)_userContext.UserId;
                newComment.CreatedDate = DateTime.Now;
                newComment.ModifiedDate = DateTime.Now;
                newComment.Type = "List";

                _listRepository.UpdateCommentCount(ListID, commentCount);

                //add notification
                if(!userID.Equals(list.UserID))
                {
                    Notification noti = new Notification();
                    User userReceive = _userRepository.GetByID(list.UserID);
                    noti.ReceiverUserId = list.UserID;
                    noti.SenderUserID = userID;
                    noti.Seen = false;
                    noti.Content = "commented your list \"" + list.ListName + "\"";
                    noti.CreatedDate = DateTime.Now;
                    noti.ModifiedDate = DateTime.Now;
                    noti.Date = DateTime.Now;
                    noti.Link = "/u/" + userReceive.UserName + "/lists/" + ListID;
                    _notificationRepository.Add(noti);
                }
               
                return _commentRepository.Add(newComment);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message);
            }
            
        }

        public int CreateCommentInReview(int ReviewID, CommentCreateDTO dto)
        {
            Guid userID = (Guid)_userContext.UserId;
            User user = _userRepository.GetByID(userID);
            if (user == null)
            {
                throw new ServiceException("Hành động không hợp lệ");
            }

            Review review = _reviewRepository.GetByID(ReviewID);
            if (review == null)
            {
                throw new ServiceException("Không tìm thấy review phù hợp");
            }
            try
            {
                int commentCount = review.CommentsCount + 1;

                Comment newComment = new Comment();
                newComment.Content = dto.Content;
                newComment.ParentID = ReviewID;
                newComment.UserID = (Guid)_userContext.UserId;
                newComment.CreatedDate = DateTime.Now;
                newComment.ModifiedDate = DateTime.Now;
                newComment.Type = "Review";

                _reviewRepository.UpdateCommentCount(ReviewID, commentCount);

                //add notification
                if (!userID.Equals(review.UserID))
                {
                    Notification noti = new Notification();
                    User userReceive = _userRepository.GetByID(review.UserID);
                    Film film = _filmRepository.GetByID(review.FilmID);

                    noti.Content = "";
                    if (film != null) {
                        noti.Content = "commented on your review of \"" + film.Title + "\"";
                    }
                    noti.ReceiverUserId = review.UserID;
                    noti.SenderUserID = userID;
                    noti.Seen = false;
                    noti.CreatedDate = DateTime.Now;
                    noti.ModifiedDate = DateTime.Now;
                    noti.Date = DateTime.Now;
                    noti.Link = "/u/" + userReceive.UserName + "/reviews/" + ReviewID;
                    _notificationRepository.Add(noti);
                }
                return _commentRepository.Add(newComment);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message);
            }
        }
    }
}
