using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.User.Profile;

namespace WebFilm.Core.Enitites.Film
{
    public class FilmDto : Film
    {
        public string Credits { get; set; }
        public int Appears { get; set; }
        public bool Liked { get; set; }
        public bool Reviewed { get; set; }
        public bool Watchlisted { get; set; }
        public RateStat RateStats { get; set; }
    }
}
