using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class WatchListDTO
    {
        public int FilmsCount { get; set; }

        public List<BaseFilmDTO> Films { get; set; }

    }
}
