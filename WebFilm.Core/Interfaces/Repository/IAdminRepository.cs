using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Admin;


namespace WebFilm.Core.Interfaces.Repository
{
    public interface IAdminRepository : IBaseRepository<int, Admin>
    {
        Task<object> GetPagingFilm(PagingParameterFilm_Admin parameter);

    }
}