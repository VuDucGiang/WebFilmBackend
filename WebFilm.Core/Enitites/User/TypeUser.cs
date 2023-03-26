using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User
{
    public enum TypeUser
    {
        All = 0,
        // Người mình đang theo dõi
        Following = 1, 
        // Người đang theo dõi mình
        Follower = 2,
        // Người bị block
        Blocked = 3,
    }
}
