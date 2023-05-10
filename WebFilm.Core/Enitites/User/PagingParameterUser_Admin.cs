using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.Film
{
    public class PagingParameterUser_Admin 
    {
        public int pageSize { get; set; } = 20;
        public int pageIndex { get; set; } = 1;
        //public string filter { get; set; } = "";
        public string? sort { get; set; }
        public string? sortBy { get; set; }
        public int? roleType { get; set; }
        public string? fullName { get; set; }
        public int? status { get; set; }
        public string? userName { get; set; }
    }
}
