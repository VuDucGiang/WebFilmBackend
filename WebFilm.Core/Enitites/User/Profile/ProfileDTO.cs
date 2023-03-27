using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class ProfileDTO
    {
        public string UserName { get; set; }

        public int ReviewCount { get; set; }

        public int ListCount { get; set; }

        public int Followers { get; set; }

        public Following Following { get; set; }

        public FavouriteFilmDTO FavouriteFilms { get; set; }

        public WatchListDTO WatchList { get; set; }

        public RecentListDTO RecentList { get; set; }

        public List<RecentLikeDTO> RecentLikes { get; set; }
    }
}
