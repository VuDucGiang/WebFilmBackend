using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Search
{
    public class FilmSearchDTO
    {
        public int FilmID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
    }
}
