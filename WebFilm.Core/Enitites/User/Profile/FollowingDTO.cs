using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class FollowingDTO
    {
        public Guid UserID { get; set; }

        public string avatar { get; set; }
    }
}
