using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class BaseFilmDTO
    {
        public int FilmID { get; set; }

        public string? Poster_path { get; set; }

        public string? Title { get; set; }

        public DateTime Release_date { get; set; }
    }
}
