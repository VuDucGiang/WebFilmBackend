using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Credit
{
    public class Credit
    {
        [Key]
        public string Credit_id { get; set; }
        public string Type { get; set; }
        public string Known_for_department { get; set; }
        public string Name { get; set; }
        public string PersonID { get; set; }
        public int FilmID { get; set; }
        public string Original_name { get; set; }
        public string Character_ { get; set; }
        public string Job { get; set; }
    }
}
