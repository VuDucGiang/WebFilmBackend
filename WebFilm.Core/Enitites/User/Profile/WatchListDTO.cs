using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class WatchListDTO
    {
        public int Total { get; set; }

        public List<BaseFilmDTO> List { get; set; }

    }
}
