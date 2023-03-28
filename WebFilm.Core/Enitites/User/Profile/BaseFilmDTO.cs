using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class BaseFilmDTO
    {
        public int ID { get; set; }

        public string PosterPath { get; set; }

        public string Title { get; set; }
    }
}
