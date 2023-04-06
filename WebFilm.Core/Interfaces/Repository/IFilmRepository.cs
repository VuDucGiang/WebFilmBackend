using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.Follow;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IFilmRepository : IBaseRepository<int, Film>
    {
        public Task<object> GetPaging(PagingParameterFilm parameter);
        Task<object> GetPopular(int pageSize, int pageIndex, string filter, string sort);
    }
}
