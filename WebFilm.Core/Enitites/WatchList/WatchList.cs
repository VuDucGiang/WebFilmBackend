using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.WatchList
{
    public class WatchList : BaseEntity
    {
        [Key]
        public int WatchListID { get; set; }
        public Guid UserID { get; set; }
        public int FilmID { get; set; }
    }
}
