using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class PagingParameterFilm : PagingParameter
    {
        public int? year { get; set; }
        public float? vote_average { get; set; }
        public string? genre { get; set; }

        public string? title { get; set; }
    }
}
