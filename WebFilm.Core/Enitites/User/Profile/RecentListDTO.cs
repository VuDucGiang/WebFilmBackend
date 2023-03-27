using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class RecentListDTO
    {
        public int listCount { get; set; }

        public List<BaseFilmDTO> films { get; set; }
    }
}
