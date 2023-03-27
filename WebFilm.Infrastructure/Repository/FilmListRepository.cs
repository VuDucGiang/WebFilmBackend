using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.FilmList;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class FilmListRepository : BaseRepository<int, FilmList>, IFilmListRepository
    {
        public FilmListRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
