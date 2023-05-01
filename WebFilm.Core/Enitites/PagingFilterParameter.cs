using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites
{
    public class PagingFilterParameter : PagingParameter
    {
        public string from { get; set; }

        public int id { get; set; }

    }
}
