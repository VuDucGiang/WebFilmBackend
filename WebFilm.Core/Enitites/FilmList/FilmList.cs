using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.FilmList
{
    public class FilmList : BaseEntity
    {
        [Key] 
        public int FilmListID { get; set; }
        public int FilmID { get; set; }
        public int ListID { get; set; }
    }
}
