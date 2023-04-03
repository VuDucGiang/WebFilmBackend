using Org.BouncyCastle.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class RateStat
    {
        public int Total { get; set; }

        public List<RateStatDTO> List { get; set; }
    }
}
