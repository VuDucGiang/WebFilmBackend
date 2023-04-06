using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites;
using WebFilm.Core.Enitites.FilmList;

namespace WebFilm.Core.Interfaces.Repository
{
    public interface IFilmListRepository : IBaseRepository<int, FilmList>
    {
    }
}
