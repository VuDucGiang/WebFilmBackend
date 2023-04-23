using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class RecentListDTO
    {
        public int Total { get; set; }

        public List<BaseFilmDTO> List { get; set; }

        public string Description { get; set; }
    }
}
