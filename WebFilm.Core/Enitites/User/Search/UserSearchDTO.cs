using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User.Search
{
    public class UserSearchDTO
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalFollowing { get; set; }
        public int TotalReviewed { get; set; }
    }
}
