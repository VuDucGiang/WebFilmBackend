using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Comment;
using WebFilm.Core.Enitites.Notification;
using WebFilm.Core.Enitites.Review;
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

        public NotificationService(INotificationRepository notificationRepository, IConfiguration configuration, IUserContext userContext) : base(notificationRepository)
        {
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _userContext = userContext;
        }

        public PagingNotificationResponse GetNotification(PagingParameter paging)
        {
            PagingNotificationResponse res = new PagingNotificationResponse();
            Guid userID = (Guid)_userContext.UserId;
            if (userID == Guid.Empty)
            {
                throw new ServiceException("Hành động không khả thi");
            }
            List<Notification> data = new List<Notification>();
            var noti = _notificationRepository.GetAll().Where(p => p.ReceiverUserId == userID);

            int totalCount = noti.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / paging.pageSize);
            noti = noti.OrderByDescending(p => p.CreatedDate).Skip((paging.pageIndex - 1) * paging.pageSize).Take(paging.pageSize);
            noti = noti.ToList();
            List<BaseCommentDTO> commentDTOs = new List<BaseCommentDTO>();
            foreach (Notification dto in noti)
            {
                data.Add(dto);
            }
            res.Data = data;
            res.TotalPage = totalPages;
            res.Total = totalCount;
            res.PageSize = paging.pageSize;
            res.PageIndex = paging.pageIndex;
            return res;
        }
    }
}
