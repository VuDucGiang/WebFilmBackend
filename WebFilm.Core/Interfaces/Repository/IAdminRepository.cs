using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Enitites.Admin;


namespace WebFilm.Core.Interfaces.Repository
{
    public interface IAdminRepository : IBaseRepository<int, Admin>
    {
        Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter);
        int UpdateFilm(int id, Film_Admin entity);
        int AddFilm(Film_Admin entity);
        int DeleteFilm(int id);

        Task<object> GetPagingUser(PagingParameterUser_Admin parameter);
        int UpdateUser(Guid id, User_Admin entity);
        int DeleteUser(Guid id);

    }
}