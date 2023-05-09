using WebFilm.Core.Enitites.Admin;
using WebFilm.Core.Enitites.Film;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IAdminService : IBaseService<int, Admin>
    {
        Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter);
        int UpdateFilm(int id, Film_Admin entity);
    }
}
