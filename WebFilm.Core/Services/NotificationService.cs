using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Notification;
using WebFilm.Core.Enitites.Review;
using WebFilm.Core.Enitites.Review.dto;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Exceptions;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class NotificationService : BaseService<int, Notification>, INotificationService
    {
        INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;
        IUserContext _userContext;
        IUserRepository _userRepository;

        public NotificationService(INotificationRepository notificationRepository, IConfiguration configuration, IUserContext userContext, IUserRepository userRepository) : base(notificationRepository)
        {
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _userContext = userContext;
            _userRepository = userRepository;
        }

        public PagingNotificationResponse GetNotification(PagingParameter paging)
        {
            PagingNotificationResponse res = new PagingNotificationResponse();
            Guid userID = (Guid)_userContext.UserId;
            if (userID == Guid.Empty)
            {
                throw new ServiceException("Hành động không khả thi");
            }
            List<NotificationRes> data = new List<NotificationRes>();
            var noti = _notificationRepository.GetAll().Where(p => p.ReceiverUserId == userID);
            
            int totalUnSeen = noti.Where(p => p.Seen == false).Count();
            int totalCount = noti.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / paging.pageSize);
            noti = noti.OrderByDescending(p => p.CreatedDate).Skip((paging.pageIndex - 1) * paging.pageSize).Take(paging.pageSize);
            noti = noti.ToList();
            List<BaseCommentDTO> commentDTOs = new List<BaseCommentDTO>();
            foreach (var dto in noti)
            {
                NotificationRes notification = new NotificationRes();
                UserReviewDTO sender = new UserReviewDTO();
                User user = _userRepository.GetByID(dto.SenderUserID);
                if (user != null)
                {
                    sender.Avatar = user.Avatar;
                    sender.UserID = user.UserID;
                    sender.FullName = user.FullName;
                    sender.UserName = user.UserName;
                }

                notification.Sender = sender;
                notification.Seen = dto.Seen;
                notification.CreatedDate = dto.CreatedDate;
                notification.ModifiedDate = dto.ModifiedDate;
                notification.Date = dto.Date;
                notification.Content = dto.Content;
                notification.Link = dto.Link;
                notification.NotificationID = dto.NotificationID;
                notification.ReceiverUserId = dto.ReceiverUserId;
                data.Add(notification);
            }
            res.Data = data;
            res.TotalPage = totalPages;
            res.Total = totalCount;
            res.PageSize = paging.pageSize;
            res.PageIndex = paging.pageIndex;
            res.TotalUnseen = totalUnSeen;
            return res;
        }

        public bool MarkAsSeen(int id)
        {
            bool res = false;
            Guid userID = (Guid)_userContext.UserId;
            if (userID == Guid.Empty)
            {
                throw new ServiceException("Hành động không khả thi");
            }

            if(id != -1)
            {
                Notification noti = _notificationRepository.GetByID(id);
                if (noti != null)
                {
                    noti.Seen = true;
                    _notificationRepository.Edit(id, noti);
                    res = true;
                }
            } else
            {
                List<Notification> notis = _notificationRepository.GetAll().Where(p => p.ReceiverUserId == userID && p.Seen == false).ToList();
                foreach (Notification noti in notis)
                {
                    noti.Seen = true;
                    _notificationRepository.Edit(noti.NotificationID, noti);
                }
                res = true;
            }
            return res;
        }
    }
}
