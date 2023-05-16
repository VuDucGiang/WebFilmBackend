using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class PagingParameterSimilar_film_Admin 
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;
        public string? sort { get; set; }
        public string? sortBy { get; set; }
        public int? DetailFilmID { get; set; }
        public int? FilmID { get; set; }
        public string? Title { get; set; }
    }
}
