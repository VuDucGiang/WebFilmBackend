using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Notification;

namespace WebFilm.Core.Interfaces.Services
{
    public interface INotificationService : IBaseService<int, Notification>
    {
        PagingNotificationResponse GetNotification(PagingParameter parameter);

        bool MarkAsSeen(int id);
    }
}
