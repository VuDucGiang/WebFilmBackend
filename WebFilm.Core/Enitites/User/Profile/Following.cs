using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class Following
    {
        public int FollowingCount { get; set; }

        public List<FollowingDTO> Followings { get; set; }
    }
}
