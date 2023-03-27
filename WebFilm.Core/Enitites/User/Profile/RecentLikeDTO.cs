using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class RecentLikeDTO
    {
        public int id { get; set; }

        public string posterPath { get; set; }

        public string title { get; set; }

        public float countRate { get; set; }
    }
}
