using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Enitites.User
{
    public class UserPopular : User
    {
        public int Reviews { get; set; }
        public int Follows { get; set; }
        public bool Followed { get; set; }
        public List<BaseFilmDTO> TopReviewFilms { get; set; }
    }
}
