using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Journal;
using WebFilm.Core.Enitites.User;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IAdminService : IBaseService<int, Admin>
    {
        Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter);
        int UpdateFilm(int id, Film_Admin entity);
        int AddFilm(Film_Admin entity);
        int DeleteFilm(int id);
        Task<object> GetPagingUser(PagingParameterUser_Admin parameter);
        int UpdateUser(Guid id, User_Admin entity);
        int DeleteUser(Guid id);
        Task<object> GetPagingJournal(PagingParameterJournal_Admin parameter);
        int UpdateJournal(int id, Journal_Admin entity);
        int AddJournal(Journal_Admin entity);
        int DeleteJournal(int id);
    }
}
