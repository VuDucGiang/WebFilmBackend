using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User
{
    public class UserDto : User
    {
        /// <summary>
        /// Mật khẩu 
        /// </summary>
        public string Password { get; set; }
    }
}
