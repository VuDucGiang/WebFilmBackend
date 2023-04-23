using Microsoft.Extensions.Configuration;
using WebFilm.Core.Enitites.Notification;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Core.Services
{
    public class NotificationService : BaseService<int, Notification>, INotificationService
    {
        INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;

        public NotificationService(INotificationRepository notificationRepository, IConfiguration configuration) : base(notificationRepository)
        {
            _notificationRepository = notificationRepository;
            _configuration = configuration;
        }

        
    }
}
