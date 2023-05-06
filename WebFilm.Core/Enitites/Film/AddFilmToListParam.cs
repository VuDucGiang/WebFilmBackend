using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class AddFilmToListParam
    {
        public int filmID { get; set; }
        public string listIDs { get; set; }
    }
}
