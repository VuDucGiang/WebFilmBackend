using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;

namespace WebFilm.Core.Interfaces.Services
{
    public interface IFilmService : IBaseService<int, Film>
    {
    }
}
