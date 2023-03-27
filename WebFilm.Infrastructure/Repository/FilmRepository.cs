using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.List;
using WebFilm.Core.Interfaces.Repository;

namespace WebFilm.Infrastructure.Repository
{
    public class FilmRepository : BaseRepository<int, Film>, IFilmRepository
    {
        public FilmRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
