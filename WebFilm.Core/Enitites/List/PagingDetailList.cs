using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites.List
{
    public class PagingDetailList : PagingParameter
    {
        public int? year { get; set; }
        public string? genre { get; set; }
        public string? filmName { get; set; }
        public string? rating { get; set; }
    }
}
