using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Profile
{
    public class ProfileInfo
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Bio { get; set; }

        public string Avatar { get; set; }

        public int TotalReview { get; set; }

        public int TotalLists { get; set; }

        public string? Banner { get; set; }

        public Following Followers { get; set; }

        public Following Following { get; set; }
    }
}
