using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.User
{
    public class ChangeInfoParam
    {

        /// <summary>
        /// Học và tên 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Học và tên 
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Học và tên 
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        public string? Bio { get; set; }

        public string? FavouriteFilmList { get; set; }
    }
}
