using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class PagingParameterCredit_Admin 
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;
       
        public string? sort { get; set; }
        public string? sortBy { get; set; }
        public string? Type { get; set; }
        public string? Known_for_department { get; set; }
        public string? Name { get; set; }
        public int? FilmID { get; set; }
        public string? Original_name { get; set; }
        public string? Character_ { get; set; }
        
    }
}
