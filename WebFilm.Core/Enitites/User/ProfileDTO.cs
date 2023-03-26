using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User
{
    public class ProfileDTO
    {
        public string UserName { get; set; }

        public List<FavouriteFilmDTO> FavouriteFilms { get; set; }
    }
}
