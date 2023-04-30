using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Similar_film
{
    public class Similar_film : BaseEntity
    {
        [Key]
        public int Similar_filmID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DetailFilmID { get; set; }
        public int FilmID { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Original_title { get; set; }
        public string Overview { get; set; }
        public string Poster_path { get; set; }

        public string Title { get; set; }


    }
}
