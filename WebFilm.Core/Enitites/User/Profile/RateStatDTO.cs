using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class RateStatDTO
    {
        public float Value { get; set; }

        public int Total { get; set; }

        public float Percent { get; set; }
    }
}
